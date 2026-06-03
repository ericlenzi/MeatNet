using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.SetMainUsuarioSucursal
{
    public class SetMainUsuarioSucursalHandler : IRequestHandler<SetMainUsuarioSucursalRequest, SetMainUsuarioSucursalResponse>
    {
        private readonly MeatContext context;

        public SetMainUsuarioSucursalHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<SetMainUsuarioSucursalResponse> Handle(SetMainUsuarioSucursalRequest request, CancellationToken cancellationToken)
        {
            var asignaciones = await this.context.UsuariosSucursales
                .Where(us => us.UsuarioId == request.UsuarioId)
                .ToListAsync(cancellationToken);

            var target = asignaciones.FirstOrDefault(us => us.Id == request.UsuarioSucursalId);
            if (target == null)
                throw new ValidationException("La asignacion no existe.");

            foreach (var us in asignaciones)
                us.esMain = us.Id == request.UsuarioSucursalId;

            await this.context.SaveChangesAsync(cancellationToken);

            return new SetMainUsuarioSucursalResponse();
        }
    }
}
