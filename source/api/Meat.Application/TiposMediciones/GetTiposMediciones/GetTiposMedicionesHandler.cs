using MediatR;
using Meat.Application.Shared;
using Meat.Domain.TiposMediciones;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMediciones.GetTiposMediciones
{
    public class GetTiposMedicionesHandler : IRequestHandler<GetTiposMedicionesRequest, GetTiposMedicionesResponse>
    {
        private readonly MeatContext context;

        public GetTiposMedicionesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposMedicionesResponse> Handle(GetTiposMedicionesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<TipoMedicion> queryable = this.context.TiposMediciones;

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.Codigo);

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTiposMedicionesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
