using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.GetDespiecesMateriales
{
    public class GetDespiecesMaterialesHandler : IRequestHandler<GetDespiecesMaterialesRequest, GetDespiecesMaterialesResponse>
    {
        private readonly MeatContext context;

        public GetDespiecesMaterialesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDespiecesMaterialesResponse> Handle(GetDespiecesMaterialesRequest request, CancellationToken cancellationToken)
        {
            var queryable =
                from d in this.context.DespiecesMateriales
                where (request.MaterialOrigenId == null || d.MaterialOrigenId == request.MaterialOrigenId)
                    && (request.Estado == null || d.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || d.MaterialOrigen.Nombre.Contains(request.Filter)
                        || d.MaterialOrigen.CodigoMaterial.Contains(request.Filter)
                        || d.MaterialDestino.Nombre.Contains(request.Filter)
                        || d.MaterialDestino.CodigoMaterial.Contains(request.Filter))
                orderby d.MaterialOrigen.Nombre, d.MaterialDestino.Nombre
                select new DespieceMaterialItem
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
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetDespiecesMaterialesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
