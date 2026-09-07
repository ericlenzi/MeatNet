using MediatR;
using Meat.Application.DespiecesMateriales.Shared;
using Meat.Application.Shared;
using Meat.Domain.DespiecesMateriales;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.CreateDespieceMaterial
{
    public class CreateDespieceMaterialHandler : IRequestHandler<CreateDespieceMaterialRequest, CreateDespieceMaterialResponse>
    {
        private readonly MeatContext context;

        public CreateDespieceMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateDespieceMaterialResponse> Handle(CreateDespieceMaterialRequest request, CancellationToken cancellationToken)
        {
            await DespieceMaterialValidacion.ValidateAsync(
                this.context, null, request.MaterialOrigenId, request.MaterialDestinoId,
                request.Cantidad, request.Rendimiento, cancellationToken);

            var entity = DespieceMaterialFactory.Create();
            entity.MaterialOrigenId = request.MaterialOrigenId;
            entity.MaterialDestinoId = request.MaterialDestinoId;
            entity.Cantidad = request.Cantidad;
            entity.Rendimiento = request.Rendimiento;
            entity.FechaActualizacion = DateTime.Now;

            this.context.DespiecesMateriales.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateDespieceMaterialResponse { Id = entity.Id };
        }
    }
}
