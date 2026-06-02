using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosHandler : IRequestHandler<GetEstablecimientosRequest, GetEstablecimientosResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetEstablecimientosHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetEstablecimientosResponse> Handle(GetEstablecimientosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Establecimiento> queryable;

            queryable = this.context.Establecimientos
                .Include(x => x.Sucursal)
                    .ThenInclude(s => s.Empresa)
                .Include(x => x.Especie)
                .Where(x => x.Sucursal.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Nombre.Contains(request.Filter) ||
                    x.CodigoEstablecimiento.Contains(request.Filter) ||
                    x.NumeroSenasa.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.CodigoEstablecimiento);

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetEstablecimientosResponse()
            {
                Data = this.mapper.Map<IEnumerable<GetEstablecimientosItem>>(data),
                TotalRows = totalRows,
            };
        }
    }
}