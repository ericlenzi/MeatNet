using Meat.Application.IngresosHaciendas; // FamiliaAlmacen
using Meat.Application.MovimientosCamaras;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EvaluacionFaena.Shared
{
    /// <summary>
    /// Motor de la Liberacion (Ciclo I paso 4): dada una jornada, resuelve que movimientos de
    /// camara hay que escribir y que piezas no se pueden liberar. No toca la base: solo calcula.
    /// Lo comparten la previsualizacion (muestra el plan y los problemas) y la liberacion
    /// efectiva (escribe el plan), de modo que las dos ven exactamente lo mismo.
    ///
    /// Por cada pieza no anulada y no liberada:
    ///   - material = Tipificacion.MaterialId (R-L1)
    ///   - sin despiece activo  -> 1 INGRESO del material a la camara de la pieza
    ///   - con despiece activo  -> cuarteo: 1 TRANSF_BAJA del origen + N TRANSF_ALTA de cada
    ///     destino, todos con el mismo TransformacionId
    ///
    /// El despiece se aplica en UN SOLO NIVEL, como especifica el manual: si un material destino
    /// tuviera a su vez despiece, no se encadena. Alcanza para los casos actuales (media res ->
    /// cuartos, res porcina -> medias reses, ninguno de cuyos destinos se vuelve a despiezar).
    /// </summary>
    public static class LiberacionCalculo
    {
        /// <summary>
        /// Tolerancia al validar que los rendimientos de un despiece sumen 1 (R-L4). Existe solo
        /// para absorber el error de redondeo del punto flotante, no para admitir mermas: si la
        /// suma se aparta de 1 se pierden o se inventan kilos, y eso se reporta como problema.
        /// </summary>
        public const double ToleranciaRendimiento = 0.001;

        public static async Task<LiberacionPlan> CalcularAsync(
            MeatContext context, Guid listaMatanzaId, Guid establecimientoId, string numeroLista, CancellationToken cancellationToken)
        {
            var plan = new LiberacionPlan();

            // Piezas de la jornada, sin las de romaneos anulados (R-L6).
            var piezas = await (
                from p in context.RomaneosPiezas
                join r in context.Romaneos on p.RomaneoId equals r.Id
                where r.ListaMatanzaId == listaMatanzaId && !r.Anulado
                orderby r.NumeroRomaneo, p.Letra
                select new PiezaLiberable
                {
                    PiezaId = p.Id,
                    RomaneoId = r.Id,
                    NumeroRomaneo = r.NumeroRomaneo,
                    NumeroGarron = r.NumeroGarron,
                    Letra = p.Letra,
                    Peso = p.Peso,
                    AlmacenDestinoId = p.AlmacenDestinoId,
                    AlmacenDestinoNombre = p.AlmacenDestino.Nombre,
                    TipificacionId = p.TipificacionId,
                    // La navegacion viene en null si la tipificacion fue dada de baja: el filtro
                    // de soft-delete la excluye del join. Hay que distinguir ese caso de "existe
                    // pero sin material", porque se corrigen de manera distinta.
                    TipificacionExiste = p.Tipificacion != null,
                    TipificacionDescripcion = p.Tipificacion.Descripcion,
                    MaterialId = p.Tipificacion.MaterialId,
                    TropaId = r.TropaId,
                    EspecieId = r.EspecieId,
                    TipoEspecieId = p.Tipificacion.TipoEspecieId,
                    YaLiberada = p.Liberado
                }).ToListAsync(cancellationToken);

            plan.PiezasYaLiberadas = piezas.Count(p => p.YaLiberada);

            // Idempotencia (R-L7): lo ya liberado se saltea, no se vuelve a generar.
            var pendientes = piezas.Where(p => !p.YaLiberada).ToList();
            plan.PiezasAProcesar = pendientes.Count;
            if (pendientes.Count == 0)
                return plan;

            // Camaras validas del establecimiento (R-L2), para no consultar una por pieza.
            var camarasValidas = await (
                from a in context.Almacenes
                join ta in context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where a.EstablecimientoId == establecimientoId
                    && ta.Familia == FamiliaAlmacen.Camara
                    && a.Activo
                select a.Id).ToListAsync(cancellationToken);
            var camaras = new HashSet<Guid>(camarasValidas);

            // Reglas de despiece activas, agrupadas por material origen.
            var despieces = await (
                from d in context.DespiecesMateriales
                where d.Activo
                select new DespieceAplicable
                {
                    MaterialOrigenId = d.MaterialOrigenId,
                    MaterialDestinoId = d.MaterialDestinoId,
                    MaterialDestinoCodigo = d.MaterialDestino.CodigoMaterial,
                    MaterialDestinoNombre = d.MaterialDestino.Nombre,
                    Cantidad = d.Cantidad,
                    Rendimiento = d.Rendimiento
                }).ToListAsync(cancellationToken);
            var despiecePorOrigen = despieces
                .GroupBy(d => d.MaterialOrigenId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Nombre del material de entrada, para que el plan sea legible sin joins extra.
            var materialesUsados = pendientes.Where(p => p.MaterialId.HasValue).Select(p => p.MaterialId.Value).Distinct().ToList();
            var materiales = await context.Materiales
                .Where(m => materialesUsados.Contains(m.Id))
                .Select(m => new { m.Id, m.CodigoMaterial, m.Nombre })
                .ToListAsync(cancellationToken);
            var materialPorId = materiales.ToDictionary(m => m.Id, m => new { m.CodigoMaterial, m.Nombre });

            var referencia = $"Liberacion Lista de Matanza N° {numeroLista}";

            foreach (var pieza in pendientes)
            {
                // La pieza quedo huerfana: se tipifico con algo que despues se dio de baja.
                if (!pieza.TipificacionExiste)
                {
                    plan.Problemas.Add(Problema(pieza,
                        $"La tipificacion '{pieza.TipificacionId}' fue dada de baja. Vuelva a tipificar la pieza."));
                    continue;
                }

                // R-L1: sin material no se sabe que producto entra a camara.
                if (!pieza.MaterialId.HasValue)
                {
                    plan.Problemas.Add(Problema(pieza,
                        $"La tipificacion '{pieza.TipificacionDescripcion}' no tiene material asignado."));
                    continue;
                }

                // R-L2: la existencia nace en la camara destino de la pieza.
                if (!camaras.Contains(pieza.AlmacenDestinoId))
                {
                    plan.Problemas.Add(Problema(pieza,
                        $"El destino '{pieza.AlmacenDestinoNombre}' no es una camara activa de este establecimiento."));
                    continue;
                }

                if (pieza.Peso <= 0)
                {
                    plan.Problemas.Add(Problema(pieza, "La pieza no tiene peso registrado."));
                    continue;
                }

                var materialId = pieza.MaterialId.Value;
                materialPorId.TryGetValue(materialId, out var material);

                // R-L4: el despiece es condicional. Sin regla activa, el material entra tal cual.
                if (!despiecePorOrigen.TryGetValue(materialId, out var reglas) || reglas.Count == 0)
                {
                    plan.Movimientos.Add(new MovimientoPlaneado
                    {
                        PiezaId = pieza.PiezaId,
                        RomaneoId = pieza.RomaneoId,
                        NumeroRomaneo = pieza.NumeroRomaneo,
                        NumeroGarron = pieza.NumeroGarron,
                        Letra = pieza.Letra,
                        TipoMovimientoId = TiposMovimientoCamara.Ingreso,
                        AlmacenId = pieza.AlmacenDestinoId,
                        AlmacenNombre = pieza.AlmacenDestinoNombre,
                        MaterialId = materialId,
                        MaterialCodigo = material?.CodigoMaterial,
                        MaterialNombre = material?.Nombre,
                        Cantidad = 1,
                        Peso = pieza.Peso,
                        TropaId = pieza.TropaId,
                        EspecieId = pieza.EspecieId,
                        TipoEspecieId = pieza.TipoEspecieId,
                        Referencia = referencia
                    });
                    continue;
                }

                // R-L4: los rendimientos deben repartir el peso completo, sin perder ni inventar kilos.
                var sumaRendimientos = reglas.Sum(r => r.Rendimiento);
                if (Math.Abs(sumaRendimientos - 1d) > ToleranciaRendimiento)
                {
                    plan.Problemas.Add(Problema(pieza,
                        $"Los rendimientos del despiece de '{material?.Nombre}' suman {sumaRendimientos:0.###} en lugar de 1."));
                    continue;
                }

                if (reglas.Any(r => r.Cantidad < 1))
                {
                    plan.Problemas.Add(Problema(pieza,
                        $"El despiece de '{material?.Nombre}' tiene una regla con cantidad menor a 1."));
                    continue;
                }

                // Cuarteo: la baja del origen y las altas de los destinos son un solo hecho.
                var transformacionId = Guid.NewGuid();

                plan.Movimientos.Add(new MovimientoPlaneado
                {
                    PiezaId = pieza.PiezaId,
                    RomaneoId = pieza.RomaneoId,
                    NumeroRomaneo = pieza.NumeroRomaneo,
                    NumeroGarron = pieza.NumeroGarron,
                    Letra = pieza.Letra,
                    TipoMovimientoId = TiposMovimientoCamara.TransformacionBaja,
                    AlmacenId = pieza.AlmacenDestinoId,
                    AlmacenNombre = pieza.AlmacenDestinoNombre,
                    MaterialId = materialId,
                    MaterialCodigo = material?.CodigoMaterial,
                    MaterialNombre = material?.Nombre,
                    Cantidad = -1,
                    Peso = -pieza.Peso,
                    TransformacionId = transformacionId,
                    TropaId = pieza.TropaId,
                    EspecieId = pieza.EspecieId,
                    TipoEspecieId = pieza.TipoEspecieId,
                    Referencia = referencia
                });

                foreach (var regla in reglas)
                {
                    // Cantidad = N produce N piezas de 1 unidad, repartiendo el peso entre ellas:
                    // cada una queda como linea propia, trazable y movible por separado.
                    var pesoPorUnidad = pieza.Peso * regla.Rendimiento / regla.Cantidad;

                    for (var i = 0; i < regla.Cantidad; i++)
                    {
                        plan.Movimientos.Add(new MovimientoPlaneado
                        {
                            PiezaId = pieza.PiezaId,
                            RomaneoId = pieza.RomaneoId,
                            NumeroRomaneo = pieza.NumeroRomaneo,
                            NumeroGarron = pieza.NumeroGarron,
                            Letra = pieza.Letra,
                            TipoMovimientoId = TiposMovimientoCamara.TransformacionAlta,
                            AlmacenId = pieza.AlmacenDestinoId,
                            AlmacenNombre = pieza.AlmacenDestinoNombre,
                            MaterialId = regla.MaterialDestinoId,
                            MaterialCodigo = regla.MaterialDestinoCodigo,
                            MaterialNombre = regla.MaterialDestinoNombre,
                            Cantidad = 1,
                            Peso = pesoPorUnidad,
                            TransformacionId = transformacionId,
                            TropaId = pieza.TropaId,
                            EspecieId = pieza.EspecieId,
                            TipoEspecieId = pieza.TipoEspecieId,
                            Referencia = referencia
                        });
                    }
                }
            }

            return plan;
        }

        private static ProblemaLiberacion Problema(PiezaLiberable pieza, string motivo)
        {
            return new ProblemaLiberacion
            {
                PiezaId = pieza.PiezaId,
                NumeroRomaneo = pieza.NumeroRomaneo,
                NumeroGarron = pieza.NumeroGarron,
                Letra = pieza.Letra,
                Motivo = motivo
            };
        }
    }
}
