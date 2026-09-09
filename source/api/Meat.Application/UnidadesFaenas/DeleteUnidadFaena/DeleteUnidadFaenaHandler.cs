using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.DeleteUnidadFaena
{
    public class DeleteUnidadFaenaHandler : IRequestHandler<DeleteUnidadFaenaRequest, DeleteUnidadFaenaResponse>
    {
        private readonly MeatContext context;

        public DeleteUnidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteUnidadFaenaResponse> Handle(DeleteUnidadFaenaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.UnidadesFaenas
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("La unidad de faena no existe.");

            // Las dos dependencias se cuentan siempre y se informan juntas. Cortar en la primera
            // hacia que el usuario resolviera las tipificaciones para recien ahi enterarse de que
            // ademas habia romaneos: dos vueltas para una respuesta que ya se sabia entera.
            var tipificaciones = await this.context.Tipificaciones
                .CountAsync(t => t.UnidadFaenaId == entity.Id, cancellationToken);

            // El romaneo guarda con que unidad se faeno cada animal: es historico y no se puede
            // dejar colgado.
            var romaneos = await this.context.Romaneos
                .CountAsync(r => r.UnidadFaenaId == entity.Id, cancellationToken);

            if (tipificaciones > 0 || romaneos > 0)
            {
                var motivos = new List<string>();

                if (tipificaciones > 0)
                    motivos.Add($"{tipificaciones} {(tipificaciones == 1 ? "tipificacion" : "tipificaciones")}");

                if (romaneos > 0)
                    motivos.Add($"{romaneos} {(romaneos == 1 ? "romaneo" : "romaneos")}");

                throw new ValidationException(
                    $"No se puede eliminar la unidad de faena porque tiene {string.Join(" y ", motivos)}. "
                    + "Si ya no se usa, desactivela en lugar de eliminarla.");
            }

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteUnidadFaenaResponse();
        }
    }
}
