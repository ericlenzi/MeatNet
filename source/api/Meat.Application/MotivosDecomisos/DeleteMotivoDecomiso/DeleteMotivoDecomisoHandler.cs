using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.MotivosDecomisos.DeleteMotivoDecomiso
{
    public class DeleteMotivoDecomisoHandler : IRequestHandler<DeleteMotivoDecomisoRequest, DeleteMotivoDecomisoResponse>
    {
        private readonly MeatContext context;

        public DeleteMotivoDecomisoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteMotivoDecomisoResponse> Handle(DeleteMotivoDecomisoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.MotivosDecomisos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El motivo de decomiso no existe.");

            // El romaneo es de una empresa y el SUPERADMIN borra parado en ADM, que no tiene
            // operacion propia: hay que saltear el query filter y reponer el soft delete a mano,
            // igual que en DeleteConformacionHandler.
            //
            // El motivo vive en dos lugares (R-E24): en el Romaneo cuando la condena es total, y
            // en la RomaneoPieza cuando el decomiso es parcial. Las dos referencias bloquean.
            var enRomaneos = await this.context.Romaneos
                .IgnoreQueryFilters()
                .AnyAsync(r => EF.Property<string>(r, "MotivoDecomisoId") == request.Codigo
                    && EF.Property<DateTime?>(r, "FechaBaja") == null, cancellationToken);

            var enPiezas = await this.context.RomaneosPiezas
                .IgnoreQueryFilters()
                .AnyAsync(p => EF.Property<string>(p, "MotivoDecomisoId") == request.Codigo
                    && EF.Property<DateTime?>(p, "FechaBaja") == null, cancellationToken);

            if (enRomaneos || enPiezas)
                throw new ValidationException(
                    "No se puede eliminar el motivo de decomiso porque tiene decomisos registrados. Desactivelo en su lugar.");

            this.context.MotivosDecomisos.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteMotivoDecomisoResponse();
        }
    }
}
