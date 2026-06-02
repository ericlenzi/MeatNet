using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Roles;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Roles.CreateRol
{
    public class CreateRolHandler : IRequestHandler<CreateRolRequest, CreateRolResponse>
    {
        private readonly MeatContext context;

        public CreateRolHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateRolResponse> Handle(CreateRolRequest request, CancellationToken cancellationToken)
        {
            if (this.context.Roles.Any(r => r.Codigo == request.Codigo))
                throw new ValidationException("Ya existe un rol con ese codigo.");

            var empresa = await this.context.Empresas.FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var rol = new Rol
            {
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Activo = request.Activo,
                EmpresaId = empresa.Id
            };

            this.context.Roles.Add(rol);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateRolResponse { Codigo = rol.Codigo };
        }
    }
}
