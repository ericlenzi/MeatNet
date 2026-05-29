using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Sucursales;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Sucursales.GetSucursales
{
    public class GetSucursalesHandler : IRequestHandler<GetSucursalesRequest, GetSucursalesResponse>
    {
        private readonly MeatContext context;

        public GetSucursalesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetSucursalesResponse> Handle(GetSucursalesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Sucursal> queryable = this.context.Sucursales
                .Where(x => x.Activo == true)
                .OrderBy(x => x.CodigoSucursal).AsQueryable();

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetSucursalesResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
