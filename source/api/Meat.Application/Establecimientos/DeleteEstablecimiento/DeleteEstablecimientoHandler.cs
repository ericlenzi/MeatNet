using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.DeleteEstablecimiento
{
    public class DeleteEstablecimientoHandler : IRequestHandler<DeleteEstablecimientoRequest, DeleteEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public DeleteEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEstablecimientoResponse> Handle(DeleteEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Establecimientos
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El establecimiento no existe");

            var tieneUsuarios = await this.context.UsuariosEstablecimientos
                .AnyAsync(ue => ue.EstablecimientoId == request.Id, cancellationToken);
            if (tieneUsuarios)
                throw new ValidationException("No se puede eliminar el establecimiento porque tiene usuarios asignados.");

            // Eliminar en cascada las especies asignadas al establecimiento
            var especies = await this.context.EstablecimientosEspecies
                .Where(ee => ee.EstablecimientoId == request.Id)
                .ToListAsync(cancellationToken);
            if (especies.Any())
                this.context.EstablecimientosEspecies.RemoveRange(especies);

            this.context.Establecimientos.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEstablecimientoResponse();
        }
    }
}
