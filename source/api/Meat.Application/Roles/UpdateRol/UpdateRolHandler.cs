using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Roles.UpdateRol
{
    public class UpdateRolHandler : IRequestHandler<UpdateRolRequest, UpdateRolResponse>
    {
        private readonly MeatContext context;

        public UpdateRolHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateRolResponse> Handle(UpdateRolRequest request, CancellationToken cancellationToken)
        {
            var rol = await this.context.Roles
                .Include(r => r.Empresa)
                .FirstOrDefaultAsync(r => r.Codigo == request.Codigo && r.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (rol == null)
                throw new ValidationException("El rol no existe.");

            rol.Nombre = request.Nombre;
            rol.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateRolResponse();
        }
    }
}
