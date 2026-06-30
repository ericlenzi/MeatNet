using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Almacenes.DeleteAlmacen
{
    public class DeleteAlmacenHandler : IRequestHandler<DeleteAlmacenRequest, DeleteAlmacenResponse>
    {
        private readonly MeatContext context;

        public DeleteAlmacenHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteAlmacenResponse> Handle(DeleteAlmacenRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Almacenes
                .Include(a => a.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(a => a.Id == request.Id
                    && a.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El corral no existe.");

            var enUso = await this.context.IngresosHaciendasUbicaciones
                .AnyAsync(u => u.AlmacenId == request.Id, cancellationToken);
            if (enUso)
                throw new ValidationException("No se puede eliminar el corral porque tiene hacienda ubicada.");

            this.context.Almacenes.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteAlmacenResponse();
        }
    }
}
