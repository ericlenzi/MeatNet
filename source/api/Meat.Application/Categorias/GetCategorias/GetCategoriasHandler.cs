using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Categorias;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Categorias.GetCategorias
{
    public class GetCategoriasHandler : IRequestHandler<GetCategoriasRequest, GetCategoriasResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetCategoriasHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetCategoriasResponse> Handle(GetCategoriasRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Categoria> queryable = this.context.Categorias
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

            return new GetCategoriasResponse
            {
                Data = this.mapper.Map<IEnumerable<GetCategoriasItem>>(data),
                TotalRows = totalRows,
            };
        }
    }
}
