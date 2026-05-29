using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.ParametrosSucursales;
using Meat.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ParametrosSucursales.GetParametrosSucursales
{
    public class GetParametrosSucursalesHandler : IRequestHandler<GetParametrosSucursalesRequest, GetParametrosSucursalesResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetParametrosSucursalesHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetParametrosSucursalesResponse> Handle(GetParametrosSucursalesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<ParametroSucursal> queryable = this.context.ParametrosSucursales
                .Include(x => x.Parametro)
                .Where(x => x.SucursalId == request.SucursalId)
                .OrderBy(x => x.ParametroId)
                .AsQueryable();

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetParametrosSucursalesResponse()
            {
                Data = this.mapper.Map<IEnumerable<GetParametrosSucursalesItem>>(data),
                TotalRows = totalRows
            };
        }
    }
}
