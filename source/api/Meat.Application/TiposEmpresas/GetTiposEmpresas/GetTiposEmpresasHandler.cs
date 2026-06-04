using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEmpresas.GetTiposEmpresas
{
    public class GetTiposEmpresasHandler : IRequestHandler<GetTiposEmpresasRequest, GetTiposEmpresasResponse>
    {
        private readonly MeatContext context;

        public GetTiposEmpresasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposEmpresasResponse> Handle(GetTiposEmpresasRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TiposEmpresas
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetTiposEmpresasResponse { Data = data };
        }
    }
}
