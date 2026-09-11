using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.DeletePuesto
{
    public class DeletePuestoHandler : IRequestHandler<DeletePuestoRequest, DeletePuestoResponse>
    {
        private readonly MeatContext context;

        public DeletePuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeletePuestoResponse> Handle(DeletePuestoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Puestos
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El puesto no existe.");

            // Las dos dependencias se cuentan juntas y se informan de una sola vez: la lista de
            // matanza declara donde se faena y el romaneo registra donde se tipifico.
            var listas = await this.context.ListasMatanzas
                .CountAsync(lm => lm.PuestoId == entity.Id, cancellationToken);

            var romaneos = await this.context.Romaneos
                .CountAsync(r => r.PuestoId == entity.Id, cancellationToken);

            if (listas > 0 || romaneos > 0)
            {
                var motivos = new List<string>();

                if (listas > 0)
                    motivos.Add($"{listas} {(listas == 1 ? "lista de matanza" : "listas de matanza")}");

                if (romaneos > 0)
                    motivos.Add($"{romaneos} {(romaneos == 1 ? "romaneo" : "romaneos")}");

                throw new ValidationException(
                    $"No se puede eliminar el puesto porque tiene {string.Join(" y ", motivos)}. "
                    + "Si ya no se usa, desactivelo en lugar de eliminarlo.");
            }

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeletePuestoResponse();
        }
    }
}
