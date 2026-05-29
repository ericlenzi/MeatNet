using MediatR;
using Meat.Domain.Roles;
using Meat.Repositories;
using System.ComponentModel.DataAnnotations;
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
                throw new ValidationException("Ya existe un rol con ese código.");

            var rol = new Rol
            {
                Codigo = request.Codigo,
                Nombre = request.Nombre,
                Activo = request.Activo
            };

            this.context.Roles.Add(rol);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateRolResponse { Codigo = rol.Codigo };
        }
    }
}
