using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.AnalisisFaena.GetAnalisisFaena
{
    /// <summary>
    /// Analisis de la jornada (Ciclo I paso 4b): rinde caliente, plan vs. real, tipificacion
    /// consolidada, dispersion de pesos y destino a camaras, todo abierto por cliente. Solo
    /// lectura (R-A1). Ver docs/manuales/AnalisisFaena.md.
    ///
    /// El rinde es CALIENTE y contra el peso VIVO DE INGRESO: no hay pesada en playa (desbaste)
    /// ni segunda pesada tras el oreo, asi que sale subestimado. La pantalla muestra esos
    /// supuestos junto al numero.
    ///
    /// Los decomisos no retocan la formula del rinde: lo condenado sale del numerador porque esa
    /// carne no existe, el animal sigue en el denominador porque se faeno, y la diferencia se
    /// informa aparte como merma sanitaria (R-A6).
    /// </summary>
    public class GetAnalisisFaenaHandler : IRequestHandler<GetAnalisisFaenaRequest, GetAnalisisFaenaResponse>
    {
        private readonly MeatContext context;

        public GetAnalisisFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetAnalisisFaenaResponse> Handle(GetAnalisisFaenaRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            // Banda de rinde esperable de la especie (R-A7). Si no esta configurada, no se avisa.
            var especie = await this.context.Especies
                .Where(e => e.Codigo == lm.EspecieId)
                .Select(e => new { e.RindeMinimo, e.RindeMaximo })
                .FirstOrDefaultAsync(cancellationToken);

            // R-A2: los romaneos anulados no son carne y quedan fuera de todo.
            var piezas = await (
                from p in this.context.RomaneosPiezas
                join r in this.context.Romaneos on p.RomaneoId equals r.Id
                join d in this.context.ListasMatanzasDetalles on r.ListaMatanzaDetalleId equals d.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Codigo
                join t in this.context.Tropas on r.TropaId equals t.Id
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                where r.ListaMatanzaId == lm.Id && !r.Anulado
                select new
                {
                    PiezaId = p.Id,
                    RomaneoId = r.Id,
                    p.Peso,
                    p.PesoFueraRango,
                    p.Liberado,
                    p.TipificacionId,
                    r.DecomisoTotal,
                    p.Decomisada,
                    MotivoTotalCodigo = r.MotivoDecomisoId,
                    MotivoTotalNombre = r.MotivoDecomiso != null ? r.MotivoDecomiso.Nombre : null,
                    MotivoPiezaCodigo = p.MotivoDecomisoId,
                    MotivoPiezaNombre = p.MotivoDecomiso != null ? p.MotivoDecomiso.Nombre : null,
                    p.PesoDecomisado,
                    TipificacionDescripcion = p.Tipificacion != null ? p.Tipificacion.Descripcion : null,
                    MaterialNombre = p.Tipificacion != null && p.Tipificacion.Material != null ? p.Tipificacion.Material.Nombre : null,
                    TipoEspecieId = te.Codigo,
                    TipoEspecieNombre = te.Nombre,
                    ClienteId = c.Id,
                    ClienteNombre = c.Nombre,
                    TropaId = t.Id
                }).ToListAsync(cancellationToken);

            // Renglones con su peso vivo promedio: la ubicacion se identifica por la misma clave
            // que el renglon (Tropa, Corral, TipoEspecie).
            var renglones = await (
                from d in this.context.ListasMatanzasDetalles
                join t in this.context.Tropas on d.TropaId equals t.Id
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join a in this.context.Almacenes on d.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Codigo
                where d.ListaMatanzaId == lm.Id
                select new
                {
                    d.Id,
                    d.TropaId,
                    t.NumeroTropa,
                    ClienteId = c.Id,
                    ClienteNombre = c.Nombre,
                    CorralNombre = a.Nombre,
                    TipoEspecieNombre = te.Nombre,
                    d.Secuencia,
                    d.Cantidad,
                    d.CantidadFaenada,
                    // Peso promedio del animal vivo al ingresar. Null si la tropa no tiene peso
                    // cargado: en ese caso el rinde de ese renglon no se calcula (R-A3).
                    PesoPromedioVivo = (
                        from u in this.context.IngresosHaciendasUbicaciones
                        where u.TropaId == d.TropaId && u.AlmacenId == d.AlmacenId && u.TipoEspecieId == d.TipoEspecieId
                        select (double?)u.PesoPromedio).FirstOrDefault()
                }).ToListAsync(cancellationToken);

            // R-A6: la carne condenada no llega a camara, asi que no entra al numerador del
            // rinde. El animal si se faeno, y por eso sigue contando como faenado y sus kilos
            // vivos quedan en el denominador: es justamente lo que hace caer el rinde, y lo que
            // la merma sanitaria explica.
            var piezasCarne = piezas.Where(p => !p.DecomisoTotal && !p.Decomisada).ToList();

            var kgFaena = piezasCarne.Sum(p => p.Peso);
            var animalesFaenados = piezas.Select(p => p.RomaneoId).Distinct().Count();

            // Tres formas de decomiso, tres cuentas distintas: la res condenada aporta todas sus
            // piezas, la media res condenada aporta su peso entero, y el recorte solo los kilos.
            var condenadasRes = piezas.Where(p => p.DecomisoTotal).ToList();
            var kgDecomisoTotal = condenadasRes.Sum(p => p.Peso);
            var animalesDecomisados = condenadasRes.Select(p => p.RomaneoId).Distinct().Count();

            var condenadasPieza = piezas.Where(p => !p.DecomisoTotal && p.Decomisada).ToList();
            var kgDecomisoPieza = condenadasPieza.Sum(p => p.Peso);

            var parciales = piezasCarne.Where(p => p.PesoDecomisado > 0 && p.MotivoPiezaCodigo != null).ToList();
            var kgDecomisoParcial = parciales.Sum(p => p.PesoDecomisado);
            var kgDecomisados = kgDecomisoTotal + kgDecomisoPieza + kgDecomisoParcial;

            // Kg vivos: se prorratea el promedio de la ubicacion por los animales faenados. Los
            // renglones sin peso de ingreso no suman y se informan aparte (R-A3).
            var renglonesConPeso = renglones.Where(r => r.PesoPromedioVivo.HasValue && r.PesoPromedioVivo.Value > 0).ToList();
            var kgVivos = renglonesConPeso.Sum(r => r.CantidadFaenada * r.PesoPromedioVivo.Value);
            var animalesSinPesoVivo = renglones
                .Where(r => !r.PesoPromedioVivo.HasValue || r.PesoPromedioVivo.Value <= 0)
                .Sum(r => r.CantidadFaenada);

            var response = new GetAnalisisFaenaResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                Fecha = lm.Fecha,
                EspecieId = lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                EstablecimientoNombre = lm.Establecimiento.Nombre,
                AnimalesFaenados = animalesFaenados,
                Piezas = piezasCarne.Count,
                KgFaena = kgFaena,
                KgVivos = kgVivos > 0 ? kgVivos : (double?)null,
                RindeCaliente = kgVivos > 0 ? Math.Round(kgFaena / kgVivos * 100, 2) : (double?)null,
                AnimalesSinPesoVivo = animalesSinPesoVivo,
                RindeMinimo = especie?.RindeMinimo,
                RindeMaximo = especie?.RindeMaximo,
                PiezasLiberadas = piezas.Count(p => p.Liberado),
                AnimalesDecomisados = animalesDecomisados,
                KgDecomisoTotal = kgDecomisoTotal,
                PiezasDecomisadas = condenadasPieza.Count,
                KgDecomisoPieza = kgDecomisoPieza,
                PiezasConDecomisoParcial = parciales.Count,
                KgDecomisoParcial = kgDecomisoParcial,
                KgDecomisados = kgDecomisados,
                MermaSanitaria = kgVivos > 0 ? Math.Round(kgDecomisados / kgVivos * 100, 2) : (double?)null
            };

            // R-A7: el rinde no se corrige ni se acota, solo se avisa. Un rinde fuera de la banda
            // de la especie casi siempre significa que el peso vivo de ingreso esta mal cargado,
            // que es el dato del que depende todo el denominador.
            response.RindeFueraDeRango = response.RindeCaliente.HasValue
                && ((especie?.RindeMinimo != null && response.RindeCaliente < especie.RindeMinimo)
                    || (especie?.RindeMaximo != null && response.RindeCaliente > especie.RindeMaximo));

            // --- Por cliente: el corte por el que se discute el resultado ---
            var kgVivosPorCliente = renglonesConPeso
                .GroupBy(r => r.ClienteId)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.CantidadFaenada * r.PesoPromedioVivo.Value));

            // Se agrupa sobre todas las piezas, condenadas incluidas: el cliente cuyos animales
            // se condenaron enteros tiene que aparecer igual, con sus kilos vivos y su merma.
            response.PorCliente = piezas
                .GroupBy(p => new { p.ClienteId, p.ClienteNombre })
                .Select(g =>
                {
                    var kg = g.Where(x => !x.DecomisoTotal && !x.Decomisada).Sum(x => x.Peso);
                    var kgCondenados = g.Where(x => x.DecomisoTotal || x.Decomisada).Sum(x => x.Peso)
                        + g.Where(x => !x.DecomisoTotal && !x.Decomisada).Sum(x => x.PesoDecomisado);
                    kgVivosPorCliente.TryGetValue(g.Key.ClienteId, out var vivos);
                    return new AnalisisClienteItem
                    {
                        ClienteId = g.Key.ClienteId,
                        ClienteNombre = g.Key.ClienteNombre,
                        AnimalesFaenados = g.Select(x => x.RomaneoId).Distinct().Count(),
                        Piezas = g.Count(x => !x.DecomisoTotal && !x.Decomisada),
                        KgFaena = kg,
                        KgVivos = vivos > 0 ? vivos : (double?)null,
                        RindeCaliente = vivos > 0 ? Math.Round(kg / vivos * 100, 2) : (double?)null,
                        ParticipacionKg = kgFaena > 0 ? Math.Round(kg / kgFaena * 100, 2) : 0,
                        KgDecomisados = kgCondenados
                    };
                })
                .OrderByDescending(x => x.KgFaena)
                .ToList();

            // --- Plan vs. real ---
            response.PlanVsReal = renglones
                .OrderBy(r => r.Secuencia)
                .Select(r => new PlanVsRealItem
                {
                    NumeroTropa = r.NumeroTropa,
                    ClienteId = r.ClienteId,
                    ClienteNombre = r.ClienteNombre,
                    CorralNombre = r.CorralNombre,
                    TipoEspecieNombre = r.TipoEspecieNombre,
                    Secuencia = r.Secuencia,
                    Planificado = r.Cantidad,
                    Faenado = r.CantidadFaenada,
                    Diferencia = r.CantidadFaenada - r.Cantidad,
                    Cumplimiento = r.Cantidad > 0 ? Math.Round((double)r.CantidadFaenada / r.Cantidad * 100, 2) : 0,
                    PesoPromedioVivo = r.PesoPromedioVivo
                })
                .ToList();

            // --- Tipificacion consolidada (la res condenada no se tipifica) ---
            response.Tipificaciones = piezasCarne
                .GroupBy(p => new { p.TipificacionId, p.TipificacionDescripcion, p.MaterialNombre })
                .Select(g =>
                {
                    var kg = g.Sum(x => x.Peso);
                    return new TipificacionConsolidadaItem
                    {
                        TipificacionId = g.Key.TipificacionId,
                        Descripcion = g.Key.TipificacionDescripcion ?? g.Key.TipificacionId?.ToString(),
                        MaterialNombre = g.Key.MaterialNombre,
                        Piezas = g.Count(),
                        KgFaena = kg,
                        PesoPromedio = Math.Round(kg / g.Count(), 2),
                        ParticipacionKg = kgFaena > 0 ? Math.Round(kg / kgFaena * 100, 2) : 0
                    };
                })
                .OrderByDescending(x => x.KgFaena)
                .ToList();

            // --- Pesos y dispersion (sobre la carne: la res condenada no se compara con nada) ---
            response.Dispersion = piezasCarne
                .GroupBy(p => new { p.TipoEspecieId, p.TipoEspecieNombre })
                .Select(g => new DispersionPesoItem
                {
                    TipoEspecieId = g.Key.TipoEspecieId,
                    TipoEspecieNombre = g.Key.TipoEspecieNombre,
                    Piezas = g.Count(),
                    PesoPromedio = Math.Round(g.Average(x => x.Peso), 2),
                    PesoMinimo = g.Min(x => x.Peso),
                    PesoMaximo = g.Max(x => x.Peso),
                    PiezasFueraRango = g.Count(x => x.PesoFueraRango)
                })
                .OrderBy(x => x.TipoEspecieNombre)
                .ToList();

            // --- Merma sanitaria por motivo: el informe que pide la inspeccion ---
            // Las tres formas se cuentan distinto y por eso van en columnas separadas: la res
            // condenada se cuenta en animales y aporta todos sus kilos; la media res condenada se
            // cuenta en piezas y aporta su peso entero; el recorte tambien se cuenta en piezas
            // pero aporta solo los kilos retirados.
            var porMotivo = new Dictionary<string, DecomisoMotivoItem>();

            foreach (var g in condenadasRes.Where(x => x.MotivoTotalCodigo != null)
                .GroupBy(x => new { Codigo = x.MotivoTotalCodigo, Nombre = x.MotivoTotalNombre }))
            {
                var item = Motivo(porMotivo, g.Key.Codigo, g.Key.Nombre);
                item.Animales += g.Select(x => x.RomaneoId).Distinct().Count();
                item.Kg += g.Sum(x => x.Peso);
            }

            foreach (var g in condenadasPieza.Where(x => x.MotivoPiezaCodigo != null)
                .GroupBy(x => new { Codigo = x.MotivoPiezaCodigo, Nombre = x.MotivoPiezaNombre }))
            {
                var item = Motivo(porMotivo, g.Key.Codigo, g.Key.Nombre);
                item.PiezasCondenadas += g.Count();
                item.Kg += g.Sum(x => x.Peso);
            }

            foreach (var g in parciales
                .GroupBy(x => new { Codigo = x.MotivoPiezaCodigo, Nombre = x.MotivoPiezaNombre }))
            {
                var item = Motivo(porMotivo, g.Key.Codigo, g.Key.Nombre);
                item.Piezas += g.Count();
                item.Kg += g.Sum(x => x.PesoDecomisado);
            }

            response.Decomisos = porMotivo.Values
                .OrderByDescending(x => x.Kg)
                .ToList();

            // --- Destino a camaras: lo que dejo la Liberacion. Vacio si todavia no se libero. ---
            var piezasIds = piezas.Select(p => p.PiezaId).ToList();
            var camaras = await (
                from mc in this.context.MovimientosCamaras
                join a in this.context.Almacenes on mc.AlmacenId equals a.Id
                join m in this.context.Materiales on mc.MaterialId equals m.Id
                where mc.RomaneoPiezaOrigenId != null && piezasIds.Contains(mc.RomaneoPiezaOrigenId.Value)
                group new { mc.Cantidad, mc.Peso } by new { AlmacenNombre = a.Nombre, MaterialNombre = m.Nombre } into g
                select new DestinoCamaraItem
                {
                    AlmacenNombre = g.Key.AlmacenNombre,
                    MaterialNombre = g.Key.MaterialNombre,
                    Cantidad = g.Sum(x => x.Cantidad),
                    Peso = g.Sum(x => x.Peso)
                }).ToListAsync(cancellationToken);

            // El material que se cuarteo por completo queda en cero: no es destino, es transito.
            response.Camaras = camaras
                .Where(c => c.Cantidad != 0)
                .OrderBy(c => c.AlmacenNombre).ThenBy(c => c.MaterialNombre)
                .ToList();

            return response;
        }

        /// <summary>Toma (o crea) la fila del motivo en el desglose de decomisos.</summary>
        private static DecomisoMotivoItem Motivo(Dictionary<string, DecomisoMotivoItem> acumulado, string codigo, string nombre)
        {
            if (!acumulado.TryGetValue(codigo, out var item))
            {
                item = new DecomisoMotivoItem { MotivoCodigo = codigo, MotivoNombre = nombre ?? codigo };
                acumulado[codigo] = item;
            }

            return item;
        }
    }
}
