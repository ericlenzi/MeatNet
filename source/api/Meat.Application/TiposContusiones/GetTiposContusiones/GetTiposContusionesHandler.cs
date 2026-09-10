using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.TiposContusiones;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposContusiones.GetTiposContusiones
{
    public class GetTiposContusionesHandler : IRequestHandler<GetTiposContusionesRequest, GetTiposContusionesResponse>
    {
        private readonly MeatContext context;

        public GetTiposContusionesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposContusionesResponse> Handle(GetTiposContusionesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<TipoContusion> queryable = this.context.TiposContusiones;

            if (!string.IsNullOrEmpty(request.EspecieId))
                queryable = queryable.Where(x => x.EspecieId == request.EspecieId);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));

            // Es una escala ordinal: se lista por Orden, no alfabeticamente.
            queryable = queryable.OrderBy(x => x.EspecieId).ThenBy(x => x.Orden);

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTiposContusionesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
