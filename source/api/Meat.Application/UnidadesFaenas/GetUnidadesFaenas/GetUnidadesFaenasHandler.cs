using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.GetUnidadesFaenas
{
    public class GetUnidadesFaenasHandler : IRequestHandler<GetUnidadesFaenasRequest, GetUnidadesFaenasResponse>
    {
        private readonly MeatContext context;

        public GetUnidadesFaenasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUnidadesFaenasResponse> Handle(GetUnidadesFaenasRequest request, CancellationToken cancellationToken)
        {
            var queryable =
                from u in this.context.UnidadesFaenas
                join e in this.context.Especies on u.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where (request.EspecieId == null || u.EspecieId == request.EspecieId)
                    && (request.Estado == null || u.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || u.Nombre.Contains(request.Filter)
                        || u.CodigoMaterial.Contains(request.Filter))
                orderby u.EspecieId, u.Numero
                select new UnidadFaenaItem
                {
                    Id = u.Id,
                    EspecieId = u.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    Numero = u.Numero,
                    Nombre = u.Nombre,
                    CantidadCuartos = u.CantidadCuartos,
                    UnidadComplementaria = u.UnidadComplementaria,
                    CodigoMaterial = u.CodigoMaterial,
                    ERP_Codigo = u.ERP_Codigo,
                    Activo = u.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetUnidadesFaenasResponse { Data = data, TotalRows = totalRows };
        }
    }
}
