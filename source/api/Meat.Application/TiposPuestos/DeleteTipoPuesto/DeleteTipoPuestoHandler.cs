using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposPuestos.DeleteTipoPuesto
{
    public class DeleteTipoPuestoHandler : IRequestHandler<DeleteTipoPuestoRequest, DeleteTipoPuestoResponse>
    {
        private readonly MeatContext context;

        public DeleteTipoPuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipoPuestoResponse> Handle(DeleteTipoPuestoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposPuestos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de puesto no existe.");

            // Lo que usa el catalogo es de una empresa y el SUPERADMIN borra parado en ADM, que no
            // tiene operacion propia: hay que saltear el query filter y reponer el soft delete a
            // mano, igual que en DeleteTipoEspecieHandler.
            var enUsoPorPuestos = await this.context.Puestos
                .IgnoreQueryFilters()
                .AnyAsync(x => EF.Property<string>(x, "TipoPuestoId") == request.Codigo
                    && EF.Property<DateTime?>(x, "FechaBaja") == null, cancellationToken);

            if (enUsoPorPuestos)
                throw new ValidationException(
                    "No se puede eliminar el tipo de puesto porque tiene puestos que lo usan. Desactivelo en su lugar.");

            this.context.TiposPuestos.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipoPuestoResponse();
        }
    }
}
