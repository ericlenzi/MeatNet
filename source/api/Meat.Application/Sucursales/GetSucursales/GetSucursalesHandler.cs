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
                .Include(x => x.Empresa)
                .Where(x => x.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Nombre.Contains(request.Filter) ||
                    x.CodigoSucursal.Contains(request.Filter) ||
                    x.Localidad.Contains(request.Filter) ||
                    x.Direccion.Contains(request.Filter));

            queryable = queryable.OrderBy(x => x.CodigoSucursal);

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
