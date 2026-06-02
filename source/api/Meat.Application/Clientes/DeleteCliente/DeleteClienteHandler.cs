using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.DeleteCliente
{
    public class DeleteClienteHandler : IRequestHandler<DeleteClienteRequest, DeleteClienteResponse>
    {
        private readonly MeatContext context;

        public DeleteClienteHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteClienteResponse> Handle(DeleteClienteRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Clientes
                .Include(x => x.EmpresaPadre)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.EmpresaPadre.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El cliente no existe");

            this.context.Clientes.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteClienteResponse();
        }
    }
}
