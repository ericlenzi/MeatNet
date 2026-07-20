using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Puestos;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.GetPuestos
{
    public class GetPuestosHandler : IRequestHandler<GetPuestosRequest, GetPuestosResponse>
    {
        private readonly MeatContext context;

        public GetPuestosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetPuestosResponse> Handle(GetPuestosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Puesto> queryable = this.context.Puestos
                .Where(x => x.EstablecimientoId == request.EstablecimientoId)
                .OrderBy(x => x.CodigoPuesto)
                .AsQueryable();

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetPuestosResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}