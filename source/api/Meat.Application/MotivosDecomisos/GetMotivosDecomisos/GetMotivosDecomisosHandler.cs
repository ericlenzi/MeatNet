using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.MotivosDecomisos;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.MotivosDecomisos.GetMotivosDecomisos
{
    public class GetMotivosDecomisosHandler : IRequestHandler<GetMotivosDecomisosRequest, GetMotivosDecomisosResponse>
    {
        private readonly MeatContext context;

        public GetMotivosDecomisosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMotivosDecomisosResponse> Handle(GetMotivosDecomisosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<MotivoDecomiso> queryable = this.context.MotivosDecomisos;

            if (!string.IsNullOrEmpty(request.EspecieId))
                queryable = queryable.Where(x => x.EspecieId == request.EspecieId);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));

            // Orden es la posicion en la lista del puesto (los motivos frecuentes primero), no una escala.
            queryable = queryable.OrderBy(x => x.EspecieId).ThenBy(x => x.Orden);

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetMotivosDecomisosResponse { Data = data, TotalRows = totalRows };
        }
    }
}
