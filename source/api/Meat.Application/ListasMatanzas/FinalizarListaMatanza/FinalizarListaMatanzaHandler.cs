using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.FinalizarListaMatanza
{
    /// <summary>
    /// Cierre de la lista (R-17): EN_EJECUCION -> FINALIZADA. Por cada renglon con
    /// sobrante (Cantidad - CantidadFaenada > 0) registra un movimiento LIBERACION;
    /// la liberacion efectiva del stock es por estado (FINALIZADA no reserva), asi
    /// que el sobrante vuelve a estar disponible para futuras planificaciones.
    /// </summary>
    public class FinalizarListaMatanzaHandler : IRequestHandler<FinalizarListaMatanzaRequest, FinalizarListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public FinalizarListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<FinalizarListaMatanzaResponse> Handle(FinalizarListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.EnEjecucion)
                throw new ValidationException("Solo se puede cerrar una lista En Ejecucion.");

            var ahora = DateTime.Now;

            // El cierre completo (liberaciones + finalizacion) es una unica version.
            entity.Version += 1;

            // R-17: liberar y auditar el sobrante por renglon. El renglon conserva la
            // Cantidad planificada (insumo del plan vs. real); lo liberado queda en el historial.
            var response = new FinalizarListaMatanzaResponse();
            foreach (var renglon in entity.Renglones.Where(r => r.Cantidad > r.CantidadFaenada).OrderBy(r => r.Secuencia))
            {
                var liberado = renglon.Cantidad - renglon.CantidadFaenada;
                response.TotalLiberado += liberado;
                response.RenglonesConSobrante += 1;

                this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    Version = entity.Version,
                    Fecha = ahora,
                    UsuarioId = request.UsuarioId,
                    TipoMovimiento = TiposMovimientoLM.Liberacion,
                    TropaId = renglon.TropaId,
                    AlmacenId = renglon.AlmacenId,
                    CantidadAnterior = renglon.Cantidad,
                    CantidadNueva = renglon.CantidadFaenada,
                    SecuenciaAnterior = renglon.Secuencia,
                    Motivo = string.IsNullOrWhiteSpace(request.Motivo)
                        ? $"Cierre: se liberan {liberado} animales planificados no faenados."
                        : $"Cierre: se liberan {liberado} animales planificados no faenados. {request.Motivo}"
                });
            }

            entity.EstadoListaMatanzaId = EstadosListaMatanza.Finalizada;
            entity.FechaFinalizacion = ahora;
            entity.FechaActualizacion = ahora;

            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = ahora,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.Finalizacion,
                Motivo = response.TotalLiberado > 0
                    ? $"Cierre de la jornada de faena. Total liberado: {response.TotalLiberado} animales en {response.RenglonesConSobrante} renglones."
                    : "Cierre de la jornada de faena. Sin sobrante: todo lo planificado fue faenado."
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return response;
        }
    }
}
