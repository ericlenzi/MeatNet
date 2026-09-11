using MediatR;
using Meat.Application.RendimientosSubproductos.Shared;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.UpdateRendimientoSubproducto
{
    public class UpdateRendimientoSubproductoHandler
        : IRequestHandler<UpdateRendimientoSubproductoRequest, UpdateRendimientoSubproductoResponse>
    {
        private readonly MeatContext context;

        public UpdateRendimientoSubproductoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateRendimientoSubproductoResponse> Handle(
            UpdateRendimientoSubproductoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.RendimientosSubproductos
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El rendimiento no existe.");

            await RendimientoSubproductoValidacion.ValidarAsync(
                this.context, request.EspecieId, request.MaterialId, request.Porcentaje, entity.Id, cancellationToken);

            entity.EspecieId = request.EspecieId;
            entity.MaterialId = request.MaterialId;
            entity.Porcentaje = request.Porcentaje;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateRendimientoSubproductoResponse();
        }
    }
}
