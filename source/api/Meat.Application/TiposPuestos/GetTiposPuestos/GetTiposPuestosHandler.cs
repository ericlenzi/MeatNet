using MediatR;
using Meat.Application.Shared;
using Meat.Domain.TiposPuestos;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposPuestos.GetTiposPuestos
{
    public class GetTiposPuestosHandler : IRequestHandler<GetTiposPuestosRequest, GetTiposPuestosResponse>
    {
        private readonly MeatContext context;

        public GetTiposPuestosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposPuestosResponse> Handle(GetTiposPuestosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<TipoPuesto> queryable = this.context.TiposPuestos;

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.Codigo);

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTiposPuestosResponse { Data = data, TotalRows = totalRows };
        }
    }
}
