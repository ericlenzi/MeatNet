using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Especies;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Especies.GetEspecies
{
    public class GetEspeciesHandler : IRequestHandler<GetEspeciesRequest, GetEspeciesResponse>
    {
        private readonly MeatContext context;

        public GetEspeciesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEspeciesResponse> Handle(GetEspeciesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Especie> queryable = this.context.Especies
                .OrderBy(x => x.Codigo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));
            }

            if (request.Estado.HasValue)
            {
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);
            }

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetEspeciesResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
