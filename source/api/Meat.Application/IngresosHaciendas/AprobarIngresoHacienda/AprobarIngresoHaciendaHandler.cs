using MediatR;
using Meat.Application.Shared;
using Meat.Application.Tropas;
using Meat.Domain.NumeradoresTropas;
using Meat.Domain.Tropas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.AprobarIngresoHacienda
{
    public class AprobarIngresoHaciendaHandler : IRequestHandler<AprobarIngresoHaciendaRequest, AprobarIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public AprobarIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AprobarIngresoHaciendaResponse> Handle(AprobarIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Pesadas)
                .Include(i => i.Ubicaciones).ThenInclude(u => u.Almacen)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.PendienteAprobacion)
                throw new ValidationException("Solo se puede aprobar un ingreso en estado Pendiente Aprobacion.");

            if (entity.Ubicaciones == null || !entity.Ubicaciones.Any())
                throw new ValidationException("El ingreso no tiene ubicaciones en corral cargadas.");

            // R11 - Clasificacion de corral segun estado de la hacienda
            foreach (var u in entity.Ubicaciones)
            {
                var tipoCorral = u.Almacen?.TipoAlmacenId;
                var esCorralCaidos = tipoCorral == TiposAlmacen.CorralCaidos;
                var esCorralMuertos = tipoCorral == TiposAlmacen.CorralMuertos;

                if (u.EstadoHaciendaId == EstadosHacienda.EnPie && (esCorralCaidos || esCorralMuertos))
                    throw new ValidationException($"La hacienda En Pie no puede ubicarse en el corral especial '{u.Almacen?.Nombre}'.");
                if (u.EstadoHaciendaId == EstadosHacienda.Caidos && !esCorralCaidos)
                    throw new ValidationException($"La hacienda Caidos debe ubicarse en un corral de tipo Caidos.");
                if (u.EstadoHaciendaId == EstadosHacienda.Muertos && !esCorralMuertos)
                    throw new ValidationException($"La hacienda Muertos debe ubicarse en un corral de tipo Muertos.");
            }

            // R8 - Capacidad de corral (tope duro). Ocupacion actual = ubicaciones de ingresos
            // ya aprobados en ese corral; se suma lo que aporta este ingreso.
            var almacenIds = entity.Ubicaciones.Select(u => u.AlmacenId).Distinct().ToList();

            var ocupacionPorCorral = await this.context.IngresosHaciendasUbicaciones
                .Where(u => almacenIds.Contains(u.AlmacenId)
                    && u.IngresoHaciendaId != entity.Id
                    && u.IngresoHacienda.EstadoIngresoId == EstadosIngreso.Aprobado)
                .GroupBy(u => u.AlmacenId)
                .Select(g => new { AlmacenId = g.Key, Total = g.Sum(x => x.Cantidad) })
                .ToDictionaryAsync(x => x.AlmacenId, x => x.Total, cancellationToken);

            foreach (var grupo in entity.Ubicaciones.GroupBy(u => u.AlmacenId))
            {
                var almacen = grupo.First().Almacen;
                var ocupacionActual = ocupacionPorCorral.TryGetValue(grupo.Key, out var oc) ? oc : 0;
                var aIngresar = grupo.Sum(u => u.Cantidad);

                if (almacen != null && ocupacionActual + aIngresar > almacen.Capacidad)
                {
                    var disponible = almacen.Capacidad - ocupacionActual;
                    throw new ValidationException(
                        $"El corral '{almacen.Nombre}' no tiene capacidad suficiente: " +
                        $"capacidad {almacen.Capacidad}, ocupado {ocupacionActual}, disponible {disponible}, se intenta ubicar {aIngresar}.");
                }
            }

            var response = new AprobarIngresoHaciendaResponse();

            // R3/R4 - Crear una Tropa por especie, con numeracion correlativa por
            // ClienteEstablecimiento + Especie, y ligar las ubicaciones a su tropa.
            var tipoEspecieIds = entity.Ubicaciones
                .Select(u => u.TipoEspecieId)
                .Where(id => id != null)
                .Distinct()
                .ToList();

            var especiePorTipo = await this.context.TiposEspecies
                .Where(te => tipoEspecieIds.Contains(te.Id))
                .ToDictionaryAsync(te => te.Id, te => te.EspecieId, cancellationToken);

            var especies = entity.Ubicaciones
                .Select(u => especiePorTipo.TryGetValue(u.TipoEspecieId ?? string.Empty, out var esp) ? esp : null)
                .Where(esp => esp != null)
                .Distinct()
                .ToList();

            foreach (var especieCodigo in especies)
            {
                var numerador = await this.context.NumeradoresTropas
                    .FirstOrDefaultAsync(nt => nt.ClienteEstablecimientoId == entity.ClienteEstablecimientoId
                        && nt.EspecieCodigo == especieCodigo, cancellationToken);

                if (numerador == null)
                {
                    numerador = new NumeradorTropa
                    {
                        Id = Guid.NewGuid(),
                        ClienteEstablecimientoId = entity.ClienteEstablecimientoId,
                        EspecieCodigo = especieCodigo,
                        UltimoNumeroTropa = 0
                    };
                    this.context.NumeradoresTropas.Add(numerador);
                }

                numerador.UltimoNumeroTropa += 1;

                var tropa = TropaFactory.Create();
                tropa.IngresoHaciendaId = entity.Id;
                tropa.ClienteEstablecimientoId = entity.ClienteEstablecimientoId;
                tropa.EspecieCodigo = especieCodigo;
                tropa.NumeroTropa = numerador.UltimoNumeroTropa;
                tropa.EstadoTropaId = EstadosTropa.Recepcionada;
                tropa.FechaRecepcion = DateTime.Now;
                this.context.Tropas.Add(tropa);

                // Trazabilidad: primer evento del ciclo de vida de la tropa
                await TropaMovimientos.RegistrarAsync(
                    this.context,
                    tropa.Id,
                    TiposMovimientoTropa.Recepcion,
                    EstadosTropa.Recepcionada,
                    $"Tropa recepcionada por aprobacion del ingreso #{entity.NumeroIngreso}.",
                    request.UsuarioId,
                    "INGRESO",
                    entity.Id,
                    cancellationToken);

                // Ligar las ubicaciones de esa especie a la tropa
                foreach (var u in entity.Ubicaciones.Where(u =>
                    especiePorTipo.TryGetValue(u.TipoEspecieId ?? string.Empty, out var esp) && esp == especieCodigo))
                {
                    u.TropaId = tropa.Id;
                }

                // Trazabilidad: ubicacion fisica de la tropa (un evento por corral / estado hacienda)
                foreach (var grupoCorral in entity.Ubicaciones
                    .Where(u => u.TropaId == tropa.Id)
                    .GroupBy(u => new { u.AlmacenId, NombreCorral = u.Almacen != null ? u.Almacen.Nombre : null, u.EstadoHaciendaId }))
                {
                    var cantidad = grupoCorral.Sum(x => x.Cantidad);
                    await TropaMovimientos.RegistrarAsync(
                        this.context,
                        tropa.Id,
                        TiposMovimientoTropa.Ubicacion,
                        EstadosTropa.Recepcionada,
                        $"Ubicada en corral {grupoCorral.Key.NombreCorral} ({EstadoHaciendaLabel(grupoCorral.Key.EstadoHaciendaId)}): {cantidad} cabezas.",
                        request.UsuarioId,
                        "INGRESO",
                        entity.Id,
                        cancellationToken);
                }

                response.Tropas.Add(new TropaGenerada
                {
                    Id = tropa.Id,
                    EspecieCodigo = especieCodigo,
                    NumeroTropa = tropa.NumeroTropa
                });
            }

            entity.EstadoIngresoId = EstadosIngreso.Aprobado;
            entity.FechaAprobacion = DateTime.Now;
            entity.UsuarioAprobacionId = request.UsuarioId;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return response;
        }

        private static string EstadoHaciendaLabel(string estadoHaciendaId)
        {
            if (estadoHaciendaId == EstadosHacienda.EnPie) return "En Pie";
            if (estadoHaciendaId == EstadosHacienda.Caidos) return "Caidos";
            if (estadoHaciendaId == EstadosHacienda.Muertos) return "Muertos";
            return estadoHaciendaId;
        }
    }
}
