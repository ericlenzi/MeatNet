using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.CancelarListaMatanza
{
    public class CancelarListaMatanzaHandler : IRequestHandler<CancelarListaMatanzaRequest, CancelarListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public CancelarListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CancelarListaMatanzaResponse> Handle(CancelarListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            // R-14: no se cancela una vez iniciada la faena
            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Borrador
                && entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada)
                throw new ValidationException("Solo se puede cancelar una lista en Borrador o Confirmada.");

            // Si estaba Confirmada, se audita (la reserva es derivada: al cambiar de estado se libera sola)
            var estabaConfirmada = entity.EstadoListaMatanzaId == EstadosListaMatanza.Confirmada;

            entity.EstadoListaMatanzaId = EstadosListaMatanza.Anulada;
            entity.FechaActualizacion = DateTime.Now;

            if (estabaConfirmada)
            {
                entity.Version += 1;
                this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    Version = entity.Version,
                    Fecha = DateTime.Now,
                    UsuarioId = request.UsuarioId,
                    TipoMovimiento = TiposMovimientoLM.Cancelacion,
                    Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? "Cancelacion de la lista de matanza." : request.Motivo
                });
            }

            await this.context.SaveChangesAsync(cancellationToken);

            return new CancelarListaMatanzaResponse();
        }
    }
}
