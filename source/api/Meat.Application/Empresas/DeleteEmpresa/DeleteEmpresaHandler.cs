using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.DeleteEmpresa
{
    public class DeleteEmpresaHandler : IRequestHandler<DeleteEmpresaRequest, DeleteEmpresaResponse>
    {
        private readonly MeatContext context;

        public DeleteEmpresaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEmpresaResponse> Handle(DeleteEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas
                .Include(x => x.Sucursales)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (empresa == null)
                throw new ValidationException("La empresa no existe");

            this.context.Empresas.Remove(empresa);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEmpresaResponse();
        }
    }
}
