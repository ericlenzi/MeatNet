using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Denticiones.DeleteDenticion
{
    public class DeleteDenticionHandler : IRequestHandler<DeleteDenticionRequest, DeleteDenticionResponse>
    {
        private readonly MeatContext context;

        public DeleteDenticionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteDenticionResponse> Handle(DeleteDenticionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Denticiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("La denticion no existe.");

            // El romaneo es de una empresa y el SUPERADMIN borra parado en ADM, que no tiene
            // operacion propia: hay que saltear el query filter y reponer el soft delete a mano,
            // igual que en DeleteConformacionHandler.
            var enUso = await this.context.Romaneos
                .IgnoreQueryFilters()
                .AnyAsync(r => EF.Property<string>(r, "DenticionId") == request.Codigo
                    && EF.Property<DateTime?>(r, "FechaBaja") == null, cancellationToken);

            if (enUso)
                throw new ValidationException(
                    "No se puede eliminar la denticion porque tiene romaneos registrados. Desactivela en su lugar.");

            this.context.Denticiones.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteDenticionResponse();
        }
    }
}
