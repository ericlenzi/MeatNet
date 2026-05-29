using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Roles.DeleteRol
{
    public class DeleteRolHandler : IRequestHandler<DeleteRolRequest, DeleteRolResponse>
    {
        private readonly MeatContext context;

        public DeleteRolHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteRolResponse> Handle(DeleteRolRequest request, CancellationToken cancellationToken)
        {
            var rol = await this.context.Roles.FirstOrDefaultAsync(r => r.Codigo == request.Codigo, cancellationToken);

            if (rol == null)
                throw new ValidationException("El rol no existe.");

            this.context.Remove(rol);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteRolResponse();
        }
    }
}
