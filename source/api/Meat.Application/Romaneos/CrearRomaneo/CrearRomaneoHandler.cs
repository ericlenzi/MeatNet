using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Application.ListasMatanzas;
using Meat.Application.Shared;
using Meat.Application.Tropas;
using Meat.Domain.Numeradores;
using Meat.Domain.Romaneos;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.CrearRomaneo
{
    /// <summary>
    /// Registra el romaneo de un animal (Ciclo I paso 3): crea Romaneo + Piezas + Mediciones,
    /// consume stock (CantidadFaenada += 1), registra la trazabilidad FAENA (grano grueso) y
    /// suma Puntos a las tipificaciones usadas. Ver docs/manuales/EjecucionFaena.md.
    /// </summary>
    public class CrearRomaneoHandler : IRequestHandler<CrearRomaneoRequest, CrearRomaneoResponse>
    {
        private readonly MeatContext context;

        public CrearRomaneoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CrearRomaneoResponse> Handle(CrearRomaneoRequest request, CancellationToken cancellationToken)
        {
            // 1) LM en ejecucion de la empresa
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId
                    && x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");
            if (lm.EstadoListaMatanzaId != EstadosListaMatanza.EnEjecucion)
                throw new ValidationException("Solo se puede romanear una lista En Ejecucion.");

            // 2) Especie soportada (Fase 2: V / P)
            if (!EspeciesFaena.EsSoportada(lm.EspecieId))
                throw new ValidationException("La ejecucion de faena solo soporta especies Vacuno (V) y Porcino (P).");

            // 3) Renglon elegido de esta LM
            var renglon = await this.context.ListasMatanzasDetalles
                .FirstOrDefaultAsync(d => d.Id == request.ListaMatanzaDetalleId
                    && d.ListaMatanzaId == lm.Id, cancellationToken);
            if (renglon == null)
                throw new ValidationException("El renglon indicado no pertenece a la lista.");

            // 4) Tope por renglon (R-E5): no faenar por encima de lo planificado
            if (renglon.Cantidad - renglon.CantidadFaenada <= 0)
                throw new ValidationException("El renglon ya alcanzo la cantidad planificada. Use faena de emergencia para ampliarlo.");

            // 5) Unidad de faena de la especie -> numero de piezas esperado
            var uf = await this.context.UnidadesFaenas
                .FirstOrDefaultAsync(u => u.Id == request.UnidadFaenaId, cancellationToken);
            if (uf == null)
                throw new ValidationException("La unidad de faena indicada no existe.");
            if (uf.EspecieId != lm.EspecieId)
                throw new ValidationException("La unidad de faena no corresponde a la especie de la lista.");

            var piezasEsperadas = RomaneoConstantes.PiezasPorAnimal(uf.CantidadCuartos);
            var piezas = request.Piezas ?? new List<PiezaRomaneoInput>();
            if (piezas.Count != piezasEsperadas)
                throw new ValidationException($"Se esperaban {piezasEsperadas} pieza(s) para la unidad de faena '{uf.Nombre}' y se recibieron {piezas.Count}.");
            if (piezas.Any(p => p.Peso <= 0))
                throw new ValidationException("El peso de cada pieza debe ser mayor a cero.");
            if (piezas.Any(p => string.IsNullOrEmpty(p.TipificacionId)))
                throw new ValidationException("Cada pieza debe tener una tipificacion.");

            // 6) Garron: unico por jornada entre romaneos no anulados
            if (request.NumeroGarron <= 0)
                throw new ValidationException("El numero de garron debe ser mayor a cero.");
            var garronRepetido = await this.context.Romaneos
                .AnyAsync(r => r.ListaMatanzaId == lm.Id && r.NumeroGarron == request.NumeroGarron && !r.Anulado, cancellationToken);
            if (garronRepetido)
                throw new ValidationException($"El garron {request.NumeroGarron} ya fue usado en esta jornada.");

            // 7) Tipificaciones validas (activas, de la empresa)
            var codigos = piezas.Select(p => p.TipificacionId).Distinct().ToList();
            var tipificaciones = await this.context.Tipificaciones
                .Where(t => codigos.Contains(t.Codigo) && t.CodigoEmpresa == request.CodigoEmpresa && t.Activo)
                .ToListAsync(cancellationToken);
            if (tipificaciones.Count != codigos.Count)
                throw new ValidationException("Una tipificacion seleccionada no existe, no esta activa o no pertenece a la empresa.");
            var tipPorCodigo = tipificaciones.ToDictionary(t => t.Codigo);

            // 8) Numerador ROMANEO por (Establecimiento, Especie): get-or-create + incremento
            var numerador = await this.context.Numeradores
                .FirstOrDefaultAsync(n => n.EstablecimientoId == lm.EstablecimientoId
                    && n.EspecieCodigo == lm.EspecieId
                    && n.TipoNumerador == RomaneoConstantes.TipoNumeradorRomaneo, cancellationToken);
            if (numerador == null)
            {
                numerador = new Numerador
                {
                    Id = Guid.NewGuid(),
                    EstablecimientoId = lm.EstablecimientoId,
                    EspecieCodigo = lm.EspecieId,
                    Codigo = RomaneoConstantes.TipoNumeradorRomaneo,
                    Descripcion = "Romaneo",
                    TipoNumerador = RomaneoConstantes.TipoNumeradorRomaneo,
                    UltimoNumero = 0,
                    Activo = true,
                    FechaActualizacion = DateTime.Now
                };
                this.context.Numeradores.Add(numerador);
            }
            numerador.UltimoNumero += 1;
            numerador.FechaActualizacion = DateTime.Now;

            // 9) Armado del romaneo (grafo: Romaneo -> Piezas -> Mediciones)
            var romaneo = RomaneoFactory.Create();
            romaneo.ListaMatanzaId = lm.Id;
            romaneo.ListaMatanzaDetalleId = renglon.Id;
            romaneo.TropaId = renglon.TropaId;
            romaneo.EspecieId = lm.EspecieId;
            romaneo.UnidadFaenaId = uf.Id;
            romaneo.NumeroGarron = request.NumeroGarron;
            romaneo.NumeroRomaneo = numerador.UltimoNumero;
            romaneo.UsuarioId = request.UsuarioId;

            romaneo.Piezas = piezas.Select((p, idx) =>
            {
                var pieza = RomaneoFactory.CreatePieza();
                pieza.RomaneoId = romaneo.Id;
                pieza.Letra = piezasEsperadas > 1 ? RomaneoConstantes.Letras[idx] : null;
                pieza.TipificacionId = p.TipificacionId;
                pieza.Peso = p.Peso;

                var medicion = RomaneoFactory.CreateMedicion();
                medicion.RomaneoPiezaId = pieza.Id;
                medicion.TipoMedicionId = RomaneoConstantes.MedicionPeso;
                medicion.Valor = p.Peso;
                pieza.Mediciones = new List<Domain.Romaneos.RomaneoPiezaMedicion> { medicion };

                return pieza;
            }).ToList();

            this.context.Romaneos.Add(romaneo);

            // 10) Consumo de stock: sube CantidadFaenada del renglon
            renglon.CantidadFaenada += 1;

            // 11) Puntos: +1 por cada tipificacion usada (cada pieza)
            foreach (var p in piezas)
            {
                var tip = tipPorCodigo[p.TipificacionId];
                tip.Puntos += 1;
                tip.FechaActualizacion = DateTime.Now;
            }

            // 12) Trazabilidad de la tropa (grano grueso)
            await RegistrarTrazabilidadAsync(lm, renglon.TropaId, request.UsuarioId, cancellationToken);

            await this.context.SaveChangesAsync(cancellationToken);

            return new CrearRomaneoResponse
            {
                Id = romaneo.Id,
                NumeroRomaneo = romaneo.NumeroRomaneo,
                NumeroGarron = romaneo.NumeroGarron
            };
        }

        /// <summary>
        /// Un evento FAENA (RECEPCIONADA) la primera vez que se faena la tropa, y otro
        /// (FAENADA) cuando se consume el ultimo animal. El detalle res-por-res vive en Romaneo.
        /// </summary>
        private async Task RegistrarTrazabilidadAsync(
            Domain.ListasMatanzas.ListaMatanza lm, Guid tropaId, Guid usuarioId, CancellationToken cancellationToken)
        {
            var hasFaenaPrevia = await this.context.TropasMovimientos
                .AnyAsync(m => m.TropaId == tropaId && m.TipoMovimiento == TiposMovimientoTropa.Faena, cancellationToken);
            if (!hasFaenaPrevia)
            {
                await TropaMovimientos.RegistrarAsync(
                    this.context, tropaId, TiposMovimientoTropa.Faena, EstadosTropa.Recepcionada,
                    $"Inicio de faena (Lista de Matanza N° {lm.NumeroLista}).",
                    usuarioId, "LISTA_MATANZA", lm.Id, cancellationToken);
            }

            // Animales En Pie recibidos de la tropa (base del consumo) vs. faenado acumulado (+ este).
            var totalUbicaciones = await (
                from u in this.context.IngresosHaciendasUbicaciones
                join i in this.context.IngresosHaciendas on u.IngresoHaciendaId equals i.Id
                where u.TropaId == tropaId
                    && i.EstadoIngresoId == EstadosIngreso.Aprobado
                    && u.EstadoHaciendaId == EstadosHacienda.EnPie
                select (int?)u.Cantidad).SumAsync(cancellationToken) ?? 0;

            var faenadoBefore = await this.context.ListasMatanzasDetalles
                .Where(d => d.TropaId == tropaId)
                .SumAsync(d => (int?)d.CantidadFaenada, cancellationToken) ?? 0;
            var faenadoAfter = faenadoBefore + 1; // este romaneo aun no persiste

            if (totalUbicaciones > 0 && faenadoAfter >= totalUbicaciones)
            {
                var tropa = await this.context.Tropas.FirstOrDefaultAsync(t => t.Id == tropaId, cancellationToken);
                if (tropa != null)
                    tropa.EstadoTropaId = EstadosTropa.Faenada;

                await TropaMovimientos.RegistrarAsync(
                    this.context, tropaId, TiposMovimientoTropa.Faena, EstadosTropa.Faenada,
                    $"Faena completa: {faenadoAfter} animales (Lista de Matanza N° {lm.NumeroLista}).",
                    usuarioId, "LISTA_MATANZA", lm.Id, cancellationToken);
            }
        }
    }
}
