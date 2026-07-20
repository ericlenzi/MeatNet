using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.ConfirmarListaMatanza
{
    public class ConfirmarListaMatanzaHandler : IRequestHandler<ConfirmarListaMatanzaRequest, ConfirmarListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public ConfirmarListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<ConfirmarListaMatanzaResponse> Handle(ConfirmarListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Borrador)
                throw new ValidationException("Solo se puede confirmar una lista en Borrador.");

            // R-09: requiere al menos un renglon
            if (entity.Renglones == null || !entity.Renglones.Any())
                throw new ValidationException("La lista de matanza no tiene renglones.");

            // R-A3: todos los renglones deben tener destino (camara) para confirmar
            if (entity.Renglones.Any(r => r.AlmacenDestinoId == null))
                throw new ValidationException("Todos los renglones deben tener un destino de faena (camara) para confirmar la lista.");

            // R-09: revalidar disponibilidad reservando contra otras LM Confirmadas / En Ejecucion
            var enPie = await ListaMatanzaStock.GetEnPieAsync(
                this.context, entity.EstablecimientoId, entity.EspecieId, cancellationToken);
            var reservado = await ListaMatanzaStock.GetReservadoAsync(
                this.context, entity.EstablecimientoId, entity.EspecieId, entity.Id, cancellationToken);

            foreach (var grupo in entity.Renglones.GroupBy(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId)))
            {
                var enP = enPie.TryGetValue(grupo.Key, out var ep) ? ep : 0;
                var res = reservado.TryGetValue(grupo.Key, out var rr) ? rr : 0;
                var disponible = enP - res;
                var total = grupo.Sum(r => r.Cantidad);
                if (total > disponible)
                    throw new ValidationException(
                        $"No hay stock disponible suficiente para una tropa/corral/categoria: se planifican {total}, " +
                        $"En Pie {enP}, reservado por otras listas {res}, disponible {disponible}.");
            }

            entity.EstadoListaMatanzaId = EstadosListaMatanza.Confirmada;
            entity.Version += 1;   // arranca en 1 en la primera confirmacion; monotonico si se re-confirma (R-13)
            entity.FechaConfirmacion = DateTime.Now;
            entity.UsuarioConfirmacionId = request.UsuarioId;
            entity.FechaActualizacion = DateTime.Now;

            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = DateTime.Now,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.Confirmacion,
                Motivo = "Confirmacion de la lista de matanza."
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return new ConfirmarListaMatanzaResponse();
        }
    }
}
