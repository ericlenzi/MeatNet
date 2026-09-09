using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Conformaciones.DeleteConformacion
{
    public class DeleteConformacionHandler : IRequestHandler<DeleteConformacionRequest, DeleteConformacionResponse>
    {
        private readonly MeatContext context;

        public DeleteConformacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteConformacionResponse> Handle(DeleteConformacionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Conformaciones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("La conformacion no existe.");

            // El romaneo es de una empresa y el SUPERADMIN borra parado en ADM, que no tiene
            // operacion propia: hay que saltear el query filter y reponer el soft delete a mano,
            // igual que en DeleteTipoEspecieHandler.
            var enUso = await this.context.Romaneos
                .IgnoreQueryFilters()
                .AnyAsync(r => EF.Property<string>(r, "ConformacionId") == request.Codigo
                    && EF.Property<DateTime?>(r, "FechaBaja") == null, cancellationToken);

            if (enUso)
                throw new ValidationException(
                    "No se puede eliminar la conformacion porque tiene romaneos registrados. Desactivela en su lugar.");

            this.context.Conformaciones.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteConformacionResponse();
        }
    }
}
