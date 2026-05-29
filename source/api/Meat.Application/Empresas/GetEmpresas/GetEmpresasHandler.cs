using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Empresas;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.GetEmpresas
{
    public class GetEmpresasHandler : IRequestHandler<GetEmpresasRequest, GetEmpresasResponse>
    {
        private readonly MeatContext context;

        public GetEmpresasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEmpresasResponse> Handle(GetEmpresasRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Empresa> queryable = this.context.Empresas
                .OrderBy(x => x.CodigoEmpresa).AsQueryable();

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetEmpresasResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
