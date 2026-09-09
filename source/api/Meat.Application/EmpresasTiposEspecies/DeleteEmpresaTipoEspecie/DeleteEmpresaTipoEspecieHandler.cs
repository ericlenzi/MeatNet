using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EmpresasTiposEspecies.DeleteEmpresaTipoEspecie
{
    public class DeleteEmpresaTipoEspecieHandler : IRequestHandler<DeleteEmpresaTipoEspecieRequest, DeleteEmpresaTipoEspecieResponse>
    {
        private readonly MeatContext context;

        public DeleteEmpresaTipoEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEmpresaTipoEspecieResponse> Handle(DeleteEmpresaTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.EmpresasTiposEspecies
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("La configuracion del tipo de especie no existe.");

            // Las tablas de operacion apuntan al codigo del catalogo, no a esta fila, asi que
            // borrarla no rompe ninguna FK: lo que rompe es la lectura, porque los combos y los
            // listados de la empresa salen de aca. Si la categoria ya se uso, se desactiva.
            var codigo = entity.TipoEspecieId;

            if (await this.context.Tipificaciones.AnyAsync(x => x.TipoEspecieId == codigo, cancellationToken))
                throw new ValidationException("No se puede quitar la categoria porque tiene tipificaciones. Desactivela en su lugar.");

            if (await this.context.ListasMatanzasDetalles.AnyAsync(x => x.TipoEspecieId == codigo, cancellationToken))
                throw new ValidationException("No se puede quitar la categoria porque se planifico en listas de matanza. Desactivela en su lugar.");

            if (await this.context.IngresosHaciendasPesadas.AnyAsync(x => x.TipoEspecieId == codigo, cancellationToken))
                throw new ValidationException("No se puede quitar la categoria porque tiene pesadas de ingreso de hacienda. Desactivela en su lugar.");

            if (await this.context.IngresosHaciendasUbicaciones.AnyAsync(x => x.TipoEspecieId == codigo, cancellationToken))
                throw new ValidationException("No se puede quitar la categoria porque tiene ubicaciones de ingreso de hacienda. Desactivela en su lugar.");

            if (await this.context.MovimientosCamaras.AnyAsync(x => x.TipoEspecieId == codigo, cancellationToken))
                throw new ValidationException("No se puede quitar la categoria porque tiene movimientos de camara. Desactivela en su lugar.");

            this.context.EmpresasTiposEspecies.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEmpresaTipoEspecieResponse();
        }
    }
}
