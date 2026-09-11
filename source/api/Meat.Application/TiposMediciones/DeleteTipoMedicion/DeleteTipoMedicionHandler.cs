using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMediciones.DeleteTipoMedicion
{
    public class DeleteTipoMedicionHandler : IRequestHandler<DeleteTipoMedicionRequest, DeleteTipoMedicionResponse>
    {
        private readonly MeatContext context;

        public DeleteTipoMedicionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipoMedicionResponse> Handle(DeleteTipoMedicionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposMediciones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de medicion no existe.");

            // Lo que usa el catalogo es de una empresa y el SUPERADMIN borra parado en ADM, que no
            // tiene operacion propia: hay que saltear el query filter y reponer el soft delete a
            // mano, igual que en DeleteTipoEspecieHandler.
            var enUsoPorPuestos = await this.context.Puestos
                .IgnoreQueryFilters()
                .AnyAsync(x => EF.Property<string>(x, "TipoMedicionId") == request.Codigo
                    && EF.Property<DateTime?>(x, "FechaBaja") == null, cancellationToken);

            if (enUsoPorPuestos)
                throw new ValidationException(
                    "No se puede eliminar el tipo de medicion porque tiene puestos que lo usan. Desactivelo en su lugar.");

            var enUsoPorRomaneos = await this.context.Romaneos
                .IgnoreQueryFilters()
                .AnyAsync(x => EF.Property<string>(x, "TipoMedicionId") == request.Codigo
                    && EF.Property<DateTime?>(x, "FechaBaja") == null, cancellationToken);

            if (enUsoPorRomaneos)
                throw new ValidationException(
                    "No se puede eliminar el tipo de medicion porque tiene romaneos que lo usan. Desactivelo en su lugar.");

            this.context.TiposMediciones.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipoMedicionResponse();
        }
    }
}
