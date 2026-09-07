using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.GetDespieceMaterial
{
    public class GetDespieceMaterialHandler : IRequestHandler<GetDespieceMaterialRequest, GetDespieceMaterialResponse>
    {
        private readonly MeatContext context;

        public GetDespieceMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDespieceMaterialResponse> Handle(GetDespieceMaterialRequest request, CancellationToken cancellationToken)
        {
            return await (
                from d in this.context.DespiecesMateriales
                where d.Id == request.Id
                select new GetDespieceMaterialResponse
                {
                    Id = d.Id,
                    MaterialOrigenId = d.MaterialOrigenId,
                    MaterialOrigenCodigo = d.MaterialOrigen.CodigoMaterial,
                    MaterialOrigenNombre = d.MaterialOrigen.Nombre,
                    MaterialDestinoId = d.MaterialDestinoId,
                    MaterialDestinoCodigo = d.MaterialDestino.CodigoMaterial,
                    MaterialDestinoNombre = d.MaterialDestino.Nombre,
                    Cantidad = d.Cantidad,
                    Rendimiento = d.Rendimiento,
                    Activo = d.Activo
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
