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
                .Where(x => x.Activo == true)
                .OrderBy(x => x.CodigoEstablecimiento).AsQueryable();

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