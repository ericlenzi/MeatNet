using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.GradosEngrasamiento.DeleteGradoEngrasamiento
{
    public class DeleteGradoEngrasamientoHandler : IRequestHandler<DeleteGradoEngrasamientoRequest, DeleteGradoEngrasamientoResponse>
    {
        private readonly MeatContext context;

        public DeleteGradoEngrasamientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteGradoEngrasamientoResponse> Handle(DeleteGradoEngrasamientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.GradosEngrasamiento
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El grado de engrasamiento no existe.");

            // El romaneo es de una empresa y el SUPERADMIN borra parado en ADM, que no tiene
            // operacion propia: hay que saltear el query filter y reponer el soft delete a mano,
            // igual que en DeleteTipoEspecieHandler.
            var enUso = await this.context.Romaneos
                .IgnoreQueryFilters()
                .AnyAsync(r => EF.Property<string>(r, "GradoEngrasamientoId") == request.Codigo
                    && EF.Property<DateTime?>(r, "FechaBaja") == null, cancellationToken);

            if (enUso)
                throw new ValidationException(
                    "No se puede eliminar el grado de engrasamiento porque tiene romaneos registrados. Desactivela en su lugar.");

            this.context.GradosEngrasamiento.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteGradoEngrasamientoResponse();
        }
    }
}
