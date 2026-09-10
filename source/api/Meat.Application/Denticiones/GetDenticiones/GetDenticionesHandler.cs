using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Denticiones;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Denticiones.GetDenticiones
{
    public class GetDenticionesHandler : IRequestHandler<GetDenticionesRequest, GetDenticionesResponse>
    {
        private readonly MeatContext context;

        public GetDenticionesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDenticionesResponse> Handle(GetDenticionesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Denticion> queryable = this.context.Denticiones;

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

            return new GetDenticionesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
