using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Parametros;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Parametros.GetParametros
{
    public class GetParametrosHandler : IRequestHandler<GetParametrosRequest, GetParametrosResponse>
    {
        private readonly MeatContext context;

        public GetParametrosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetParametrosResponse> Handle(GetParametrosRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Parametro> queryable = this.context.Parametros
                .Include(x => x.Empresa)
                .Where(x => x.Empresa.CodigoEmpresa == request.CodigoEmpresa)
                .OrderBy(x => x.Codigo)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                queryable = queryable.Where(x =>
                    x.Codigo.Contains(request.Filter) ||
                    x.Nombre.Contains(request.Filter) ||
                    x.Valor.Contains(request.Filter));
            }

            if (request.Estado.HasValue)
            {
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);
            }

            var totalRows = await queryable.CountAsync();

            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

            return new GetParametrosResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
