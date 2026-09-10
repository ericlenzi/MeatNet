using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposContusiones.DeleteTipoContusion
{
    public class DeleteTipoContusionHandler : IRequestHandler<DeleteTipoContusionRequest, DeleteTipoContusionResponse>
    {
        private readonly MeatContext context;

        public DeleteTipoContusionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipoContusionResponse> Handle(DeleteTipoContusionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposContusiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de contusion no existe.");

            // El romaneo es de una empresa y el SUPERADMIN borra parado en ADM, que no tiene
            // operacion propia: hay que saltear el query filter y reponer el soft delete a mano,
            // igual que en DeleteConformacionHandler.
            var enUso = await this.context.RomaneosPiezas
                .IgnoreQueryFilters()
                .AnyAsync(r => EF.Property<string>(r, "TipoContusionId") == request.Codigo
                    && EF.Property<DateTime?>(r, "FechaBaja") == null, cancellationToken);

            if (enUso)
                throw new ValidationException(
                    "No se puede eliminar el tipo de contusion porque tiene romaneos registrados. Desactivelo en su lugar.");

            this.context.TiposContusiones.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipoContusionResponse();
        }
    }
}
