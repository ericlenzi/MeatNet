using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Materiales.GetMateriales
{
    public class GetMaterialesHandler : IRequestHandler<GetMaterialesRequest, GetMaterialesResponse>
    {
        private readonly MeatContext context;

        public GetMaterialesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMaterialesResponse> Handle(GetMaterialesRequest request, CancellationToken cancellationToken)
        {
            var queryable =
                from m in this.context.Materiales
                join tm in this.context.TiposMateriales on m.TipoMaterialId equals tm.Codigo into tmj
                from tm in tmj.DefaultIfEmpty()
                join um in this.context.UnidadesMedidas on m.UnidadMedidaId equals um.Codigo into umj
                from um in umj.DefaultIfEmpty()
                where (request.TipoMaterialId == null || m.TipoMaterialId == request.TipoMaterialId)
                    && (request.Estado == null || m.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || m.Nombre.Contains(request.Filter)
                        || m.CodigoMaterial.Contains(request.Filter)
                        || m.ERP_Codigo.Contains(request.Filter))
                orderby m.Nombre
                select new MaterialItem
                {
                    Id = m.Id,
                    CodigoMaterial = m.CodigoMaterial,
                    Nombre = m.Nombre,
                    TipoMaterialId = m.TipoMaterialId,
                    TipoMaterialNombre = tm != null ? tm.Nombre : null,
                    UnidadMedidaId = m.UnidadMedidaId,
                    UnidadMedidaNombre = um != null ? um.Nombre : null,
                    PesoTeorico = m.PesoTeorico,
                    ERP_Codigo = m.ERP_Codigo,
                    Activo = m.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetMaterialesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
