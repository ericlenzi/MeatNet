using MediatR;
using Meat.Application.RendimientosSubproductos.Shared;
using Meat.Domain.RendimientosSubproductos;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.CreateRendimientoSubproducto
{
    public class CreateRendimientoSubproductoHandler
        : IRequestHandler<CreateRendimientoSubproductoRequest, CreateRendimientoSubproductoResponse>
    {
        private readonly MeatContext context;

        public CreateRendimientoSubproductoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateRendimientoSubproductoResponse> Handle(
            CreateRendimientoSubproductoRequest request, CancellationToken cancellationToken)
        {
            await RendimientoSubproductoValidacion.ValidarAsync(
                this.context, request.EspecieId, request.MaterialId, request.Porcentaje, null, cancellationToken);

            var entity = RendimientoSubproductoFactory.Create();
            entity.EspecieId = request.EspecieId;
            entity.MaterialId = request.MaterialId;
            entity.Porcentaje = request.Porcentaje;

            this.context.RendimientosSubproductos.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateRendimientoSubproductoResponse { Id = entity.Id };
        }
    }
}
