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
                .Include(x => x.Establecimiento)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
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

            // 5c) Los cuatro datos del palco (R-E20). La obligatoriedad no esta escrita por
            //     especie: sale del propio catalogo. Si la especie de la jornada tiene valores
            //     activos, el dato es obligatorio; si no los tiene, el Tipificador ni siquiera
            //     muestra el combo y el campo tiene que venir vacio. Asi el vacuno los exige y el
            //     porcino no, sin ningun codigo de especie clavado en la logica (R-E21).
            var conformaciones = await this.context.Conformaciones
                .Where(c => c.Activo && c.EspecieId == lm.EspecieId)
                .Select(c => c.Codigo).ToListAsync(cancellationToken);
            var gradosEngrasamiento = await this.context.GradosEngrasamiento
                .Where(g => g.Activo && g.EspecieId == lm.EspecieId)
                .Select(g => g.Codigo).ToListAsync(cancellationToken);
            var denticiones = await this.context.Denticiones
                .Where(d => d.Activo && d.EspecieId == lm.EspecieId)
                .Select(d => d.Codigo).ToListAsync(cancellationToken);
            var escalaContusiones = await this.context.TiposContusiones
                .Where(t => t.Activo && t.EspecieId == lm.EspecieId)
                .OrderBy(t => t.Orden)
                .Select(t => t.Codigo).ToListAsync(cancellationToken);
            var contusiones = escalaContusiones;

            // La primera de la escala es "sin contusion" (Orden mas bajo): la usa R-E26 para
            // detectar la media res que se declara sana y a la vez se decomisa por un golpe.
            var contusionSana = escalaContusiones.FirstOrDefault();

            var motivosDecomiso = await this.context.MotivosDecomisos
                .Where(m => m.Activo && m.EspecieId == lm.EspecieId)
                .Select(m => new { m.Codigo, m.ExigeContusion })
                .ToDictionaryAsync(m => m.Codigo, m => m.ExigeContusion, cancellationToken);

            // 5d) Decomiso total (R-E23): la inspeccion condena la res entera. El motivo es
            //     obligatorio, y la res condenada no se clasifica: no se piden los datos del
            //     palco ni la tipificacion, porque no va a ser carne. Si el puesto los manda
            //     igual, se descartan aca en vez de frenar la linea con un error.
            var decomisoTotal = request.DecomisoTotal;
            var motivoTotal = Normalizar(request.MotivoDecomisoId);

            if (decomisoTotal)
            {
                if (motivoTotal == null)
                    throw new ValidationException("Debe indicar el motivo por el que se condena la res.");
                if (!motivosDecomiso.ContainsKey(motivoTotal))
                    throw new ValidationException("El motivo de decomiso indicado no existe, no esta activo o no corresponde a la especie de la jornada.");
            }
            else
            {
                if (motivoTotal != null)
                    throw new ValidationException("El motivo de decomiso de la res solo se registra cuando se la condena entera.");

                ValidarDatoDePalco("la conformacion de la res", conformaciones, request.ConformacionId);
                ValidarDatoDePalco("el grado de engrasamiento de la res", gradosEngrasamiento, request.GradoEngrasamientoId);
                ValidarDatoDePalco("la denticion del animal", denticiones, request.DenticionId);
            }

            var piezasEsperadas = Math.Max(1, uf.PiezasPorAnimal);
            var piezas = request.Piezas ?? new List<PiezaRomaneoInput>();
            if (piezas.Count != piezasEsperadas)
                throw new ValidationException($"Se esperaban {piezasEsperadas} pieza(s) para la unidad de faena '{uf.Nombre}' y se recibieron {piezas.Count}.");
            if (piezas.Any(p => p.Peso <= 0))
                throw new ValidationException("El peso de cada pieza debe ser mayor a cero.");

            // La res condenada se pesa igual: esos kilos son la merma sanitaria de la jornada.
            // Lo que no lleva es tipificacion ni contusion.
            if (!decomisoTotal && piezas.Any(p => !p.TipificacionId.HasValue))
                throw new ValidationException("Cada pieza debe tener una tipificacion.");

            // La contusion es el unico de los cuatro datos del palco que va por pieza: el golpe
            // esta en una media res concreta. Se valida con la misma regla derivada del catalogo.
            if (!decomisoTotal)
            {
                foreach (var p in piezas)
                    ValidarDatoDePalco("la contusion de cada media res", contusiones, p.TipoContusionId);
            }

            // 5e) Decomiso parcial (R-E24): la inspeccion retira kilos de una media res y la
            //     pieza sigue su curso a camara. Motivo y kilos van juntos, y los kilos no pueden
            //     alcanzar el peso de la pieza: eso ya es una condena y va por decomiso total.
            foreach (var p in piezas)
            {
                var motivoPieza = Normalizar(p.MotivoDecomisoId);

                if (decomisoTotal)
                {
                    if (motivoPieza != null || p.PesoDecomisado > 0)
                        throw new ValidationException("La res condenada entera no lleva ademas decomisos parciales por pieza.");
                    continue;
                }

                if (motivoPieza == null && p.PesoDecomisado <= 0)
                    continue;
                if (motivoPieza == null)
                    throw new ValidationException("Indique el motivo del decomiso parcial de la pieza.");
                if (!motivosDecomiso.TryGetValue(motivoPieza, out var exigeContusion))
                    throw new ValidationException("El motivo de decomiso indicado no existe, no esta activo o no corresponde a la especie de la jornada.");
                if (p.PesoDecomisado <= 0)
                    throw new ValidationException("Los kilos decomisados de la pieza deben ser mayores a cero.");
                if (p.PesoDecomisado >= p.Peso)
                    throw new ValidationException("Los kilos decomisados no pueden alcanzar el peso de la pieza. Si se condena la res entera, use el decomiso total.");

                // R-E26: el motivo que describe un golpe no cierra con una media res declarada
                // sana. Quien decide cuales son esos motivos es el catalogo (ExigeContusion), no
                // una lista de codigos escrita aca.
                if (exigeContusion && contusionSana != null
                    && (string.IsNullOrWhiteSpace(p.TipoContusionId) || p.TipoContusionId == contusionSana))
                {
                    throw new ValidationException(
                        "El decomiso es por contusion, asi que la media res no puede quedar registrada sin contusion. Indique la contusion que corresponde.");
                }
            }

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

            // 7) Tipificaciones validas (activas, de la empresa). La res condenada no tiene:
            //    la lista queda vacia y todo este bloque se saltea solo.
            var tipIds = decomisoTotal
                ? new List<Guid>()
                : piezas.Select(p => p.TipificacionId.Value).Distinct().ToList();
            var tipificaciones = await this.context.Tipificaciones
                .Where(t => tipIds.Contains(t.Id) && t.Activo)
                .ToListAsync(cancellationToken);
            if (tipificaciones.Count != tipIds.Count)
                throw new ValidationException("Una tipificacion seleccionada no existe, no esta activa o no pertenece a la empresa.");
            var tipPorId = tipificaciones.ToDictionary(t => t.Id);

            // 7b) Peso dentro del rango de su tipificacion. Fuera de rango no bloquea la linea:
            // se permite si el operario lo confirma (ForzarFueraRango) y queda registrado en la pieza.
            foreach (var p in piezas)
            {
                if (decomisoTotal) break;
                var t = tipPorId[p.TipificacionId.Value];
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
            romaneo.UnidadFaenaId = uf.Id;
            romaneo.ConformacionId = string.IsNullOrWhiteSpace(request.ConformacionId) ? null : request.ConformacionId;
            romaneo.GradoEngrasamientoId = string.IsNullOrWhiteSpace(request.GradoEngrasamientoId) ? null : request.GradoEngrasamientoId;
            romaneo.DenticionId = string.IsNullOrWhiteSpace(request.DenticionId) ? null : request.DenticionId;
            romaneo.DecomisoTotal = decomisoTotal;
            romaneo.MotivoDecomisoId = motivoTotal;

            // La res condenada no se clasifica (R-E23): lo que haya llegado del puesto se descarta.
            if (decomisoTotal)
            {
                romaneo.ConformacionId = null;
                romaneo.GradoEngrasamientoId = null;
                romaneo.DenticionId = null;
            }
            romaneo.NumeroGarron = request.NumeroGarron;
            romaneo.NumeroRomaneo = numeroRomaneo;
            romaneo.UsuarioId = request.UsuarioId;

            romaneo.Piezas = piezas.Select((p, idx) =>
            {
                var pieza = RomaneoFactory.CreatePieza();
                pieza.RomaneoId = romaneo.Id;
                pieza.Letra = piezasEsperadas > 1 ? RomaneoConstantes.Letras[idx] : null;
                pieza.AlmacenDestinoId = p.AlmacenDestinoId;
                pieza.TipificacionId = decomisoTotal ? null : p.TipificacionId;
                pieza.TipoContusionId = decomisoTotal || string.IsNullOrWhiteSpace(p.TipoContusionId) ? null : p.TipoContusionId;
                pieza.Peso = p.Peso;
                pieza.MotivoDecomisoId = decomisoTotal ? null : Normalizar(p.MotivoDecomisoId);
                pieza.PesoDecomisado = decomisoTotal ? 0 : p.PesoDecomisado;

                // Sin tipificacion no hay rango contra el cual comparar el peso.
                var tipPieza = decomisoTotal ? null : tipPorId[p.TipificacionId.Value];
                pieza.PesoFueraRango = tipPieza != null && (p.Peso < tipPieza.PesoDesde || p.Peso > tipPieza.PesoHasta);

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

            // 11) Puntos: +1 por cada tipificacion usada (cada pieza). La res condenada no usa
            //     ninguna, asi que no mueve la propuesta del proximo romaneo.
            foreach (var p in piezas)
            {
                if (decomisoTotal) break;
                var tip = tipPorId[p.TipificacionId.Value];
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

        /// <summary>Recorta el codigo y devuelve null si quedo vacio.</summary>
        private static string Normalizar(string valor)
            => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

        /// <summary>
        /// Valida uno de los cuatro datos del palco contra su catalogo (R-E20). El catalogo es la
        /// regla: si la especie tiene valores activos el dato es obligatorio, y si no los tiene
        /// tiene que venir vacio. No hay ninguna especie escrita a mano en esta logica, asi que
        /// sumar ovinos o caprinos es cargar filas, no tocar codigo.
        /// </summary>
        private static void ValidarDatoDePalco(string nombre, List<string> codigosDeLaEspecie, string valor)
        {
            var vacio = string.IsNullOrWhiteSpace(valor);

            if (codigosDeLaEspecie.Count == 0)
            {
                if (!vacio)
                    throw new ValidationException($"La especie de la jornada no registra {nombre}.");
                return;
            }

            if (vacio)
                throw new ValidationException($"Debe indicar {nombre}.");

            if (!codigosDeLaEspecie.Contains(valor))
                throw new ValidationException($"El valor indicado para {nombre} no existe, no esta activo o no corresponde a la especie de la jornada.");
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
