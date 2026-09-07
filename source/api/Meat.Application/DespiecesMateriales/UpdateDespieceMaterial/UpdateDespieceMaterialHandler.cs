using MediatR;
using Meat.Application.DespiecesMateriales.Shared;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.UpdateDespieceMaterial
{
    public class UpdateDespieceMaterialHandler : IRequestHandler<UpdateDespieceMaterialRequest, UpdateDespieceMaterialResponse>
    {
        private readonly MeatContext context;

        public UpdateDespieceMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateDespieceMaterialResponse> Handle(UpdateDespieceMaterialRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.DespiecesMateriales
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El despiece no existe.");

            await DespieceMaterialValidacion.ValidateAsync(
                this.context, request.Id, request.MaterialOrigenId, request.MaterialDestinoId,
                request.Cantidad, request.Rendimiento, cancellationToken);

            entity.MaterialOrigenId = request.MaterialOrigenId;
            entity.MaterialDestinoId = request.MaterialDestinoId;
            entity.Cantidad = request.Cantidad;
            entity.Rendimiento = request.Rendimiento;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateDespieceMaterialResponse();
        }
    }
}
