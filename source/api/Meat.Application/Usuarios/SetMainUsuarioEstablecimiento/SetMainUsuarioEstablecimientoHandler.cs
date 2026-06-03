using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.SetMainUsuarioEstablecimiento
{
    public class SetMainUsuarioEstablecimientoHandler : IRequestHandler<SetMainUsuarioEstablecimientoRequest, SetMainUsuarioEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public SetMainUsuarioEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<SetMainUsuarioEstablecimientoResponse> Handle(SetMainUsuarioEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var asignaciones = await this.context.UsuariosEstablecimientos
                .Where(ue => ue.UsuarioId == request.UsuarioId)
                .ToListAsync(cancellationToken);

            var target = asignaciones.FirstOrDefault(ue => ue.Id == request.UsuarioEstablecimientoId);
            if (target == null)
                throw new ValidationException("La asignacion no existe.");

            foreach (var ue in asignaciones)
                ue.EsMain = ue.Id == request.UsuarioEstablecimientoId;

            await this.context.SaveChangesAsync(cancellationToken);

            return new SetMainUsuarioEstablecimientoResponse();
        }
    }
}
