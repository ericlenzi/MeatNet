using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.DesconfirmarListaMatanza
{
    /// <summary>
    /// R-13: vuelve una LM de CONFIRMADA a BORRADOR. La reserva se libera sola
    /// (es derivada del estado). Solo si ningun renglon tiene faena registrada.
    /// </summary>
    public class DesconfirmarListaMatanzaHandler : IRequestHandler<DesconfirmarListaMatanzaRequest, DesconfirmarListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public DesconfirmarListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DesconfirmarListaMatanzaResponse> Handle(DesconfirmarListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada)
                throw new ValidationException("Solo se puede volver a Borrador una lista Confirmada.");

            // R-13: no se puede desconfirmar si ya hay faena registrada
            if (entity.Renglones.Any(r => r.CantidadFaenada > 0))
                throw new ValidationException("No se puede volver a Borrador: la lista tiene renglones con faena registrada.");

            entity.EstadoListaMatanzaId = EstadosListaMatanza.Borrador;
            entity.Version += 1;
            entity.FechaConfirmacion = null;
            entity.UsuarioConfirmacionId = null;
            entity.FechaActualizacion = DateTime.Now;

            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = DateTime.Now,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.Desconfirmacion,
                Motivo = "Vuelta a Borrador: se libera la reserva de stock."
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return new DesconfirmarListaMatanzaResponse();
        }
    }
}
