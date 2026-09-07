using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Materiales.GetMaterial
{
    public class GetMaterialHandler : IRequestHandler<GetMaterialRequest, GetMaterialResponse>
    {
        private readonly MeatContext context;

        public GetMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMaterialResponse> Handle(GetMaterialRequest request, CancellationToken cancellationToken)
        {
            return await (
                from m in this.context.Materiales
                join tm in this.context.TiposMateriales on m.TipoMaterialId equals tm.Codigo into tmj
                from tm in tmj.DefaultIfEmpty()
                join um in this.context.UnidadesMedidas on m.UnidadMedidaId equals um.Codigo into umj
                from um in umj.DefaultIfEmpty()
                where m.Id == request.Id
                select new GetMaterialResponse
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
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
