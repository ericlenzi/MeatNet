using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.TiposEspecies;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEspecies.GetTiposEspecies
{
    public class GetTiposEspeciesHandler : IRequestHandler<GetTiposEspeciesRequest, GetTiposEspeciesResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetTiposEspeciesHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetTiposEspeciesResponse> Handle(GetTiposEspeciesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<TipoEspecie> queryable = this.context.TiposEspecies
                .Include(x => x.Especie)
                .Include(x => x.TipoSexo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.EspecieId))
                queryable = queryable.Where(x => x.EspecieId == request.EspecieId);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Id.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.EspecieId).ThenBy(x => x.Nombre);

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTiposEspeciesResponse
            {
                Data = this.mapper.Map<IEnumerable<GetTiposEspeciesItem>>(data),
                TotalRows = totalRows,
            };
        }
    }
}
