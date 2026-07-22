using MediatR;
using Meat.Application.IngresosHaciendas; // TiposAlmacen / FamiliaAlmacen
using Meat.Application.ListasMatanzas;
using Meat.Application.Numeradores;
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
                .FirstOrDefaultAsync(u => u.Codigo == request.UnidadFaenaId, cancellationToken);
            if (uf == null)
                throw new ValidationException("La unidad de faena indicada no existe.");
            if (uf.EspecieId != lm.EspecieId)
                throw new ValidationException("La unidad de faena no corresponde a la especie de la lista.");

            var piezasEsperadas = Math.Max(1, uf.PiezasPorAnimal);
            var piezas = request.Piezas ?? new List<PiezaRomaneoInput>();
            if (piezas.Count != piezasEsperadas)
                throw new ValidationException($"Se esperaban {piezasEsperadas} pieza(s) para la unidad de faena '{uf.Nombre}' y se recibieron {piezas.Count}.");
            if (piezas.Any(p => p.Peso <= 0))
                throw new ValidationException("El peso de cada pieza debe ser mayor a cero.");
            if (piezas.Any(p => string.IsNullOrEmpty(p.TipificacionId)))
                throw new ValidationException("Cada pieza debe tener una tipificacion.");

            // 5b) Camara destino por pieza (default del renglon, editable en el puesto): obligatoria
            //     y valida (camara activa del establecimiento de la LM). Cada media res puede ir a
            //     una camara distinta; se valida el conjunto de camaras usado.
            if (piezas.Any(p => p.AlmacenDestinoId == Guid.Empty))
                throw new ValidationException("Cada pieza debe tener una camara de destino.");
            var camarasUsadas = piezas.Select(p => p.AlmacenDestinoId).Distinct().ToList();
            var camarasValidas = await (
                from a in this.context.Almacenes
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where camarasUsadas.Contains(a.Id)
                    && a.EstablecimientoId == lm.EstablecimientoId
                    && ta.Familia == FamiliaAlmacen.Camara
                    && a.Activo
                select a.Id).ToListAsync(cancellationToken);
            if (camarasValidas.Count != camarasUsadas.Count)
                throw new ValidationException("Una camara de destino no es una camara activa de este establecimiento.");

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

            // 7b) Peso dentro del rango de su tipificacion. Fuera de rango no bloquea la linea:
            // se permite si el operario lo confirma (ForzarFueraRango) y queda registrado en la pieza.
            foreach (var p in piezas)
            {
                var t = tipPorCodigo[p.TipificacionId];
                if (p.Peso >= t.PesoDesde && p.Peso <= t.PesoHasta) continue;
                if (!p.ForzarFueraRango)
                    throw new ValidationException($"El peso {p.Peso} kg esta fuera del rango {t.PesoDesde}-{t.PesoHasta} kg de la tipificacion '{t.Descripcion}'. Confirme el registro para forzarlo.");
            }

            // 8) Numerador ROMANEO por (Establecimiento, Especie): reserva atomica del proximo numero.
            // La reserva y el alta van en la misma transaccion: si algo falla despues, el correlativo
            // se revierte y no queda hueco.
            await using var tx = await this.context.Database.BeginTransactionAsync(cancellationToken);
            var numeroRomaneo = await Correlativos.ReservarAsync(
                this.context, lm.EstablecimientoId, lm.EspecieId,
                TiposNumerador.Romaneo, "Romaneo", cancellationToken);

            // 9) Armado del romaneo (grafo: Romaneo -> Piezas -> Mediciones)
            var romaneo = RomaneoFactory.Create();
            romaneo.ListaMatanzaId = lm.Id;
            romaneo.EstablecimientoId = lm.EstablecimientoId;
            romaneo.ListaMatanzaDetalleId = renglon.Id;
            romaneo.TropaId = renglon.TropaId;
            romaneo.EspecieId = lm.EspecieId;
            romaneo.UnidadFaenaId = uf.Codigo;
            romaneo.NumeroGarron = request.NumeroGarron;
            romaneo.NumeroRomaneo = numeroRomaneo;
            romaneo.UsuarioId = request.UsuarioId;

            romaneo.Piezas = piezas.Select((p, idx) =>
            {
                var pieza = RomaneoFactory.CreatePieza();
                pieza.RomaneoId = romaneo.Id;
                pieza.Letra = piezasEsperadas > 1 ? RomaneoConstantes.Letras[idx] : null;
                pieza.AlmacenDestinoId = p.AlmacenDestinoId;
                pieza.TipificacionId = p.TipificacionId;
                pieza.Peso = p.Peso;
                var tipPieza = tipPorCodigo[p.TipificacionId];
                pieza.PesoFueraRango = p.Peso < tipPieza.PesoDesde || p.Peso > tipPieza.PesoHasta;

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
            await tx.CommitAsync(cancellationToken);

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
