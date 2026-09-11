using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.DeleteTipificador
{
    public class DeleteTipificadorHandler : IRequestHandler<DeleteTipificadorRequest, DeleteTipificadorResponse>
    {
        private readonly MeatContext context;

        public DeleteTipificadorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipificadorResponse> Handle(DeleteTipificadorRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Tipificadores
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El tipificador no existe.");

            // El romaneo guarda quien tipifico: es historico y no se puede dejar colgado.
            var romaneos = await this.context.Romaneos
                .CountAsync(r => r.TipificadorId == entity.Id, cancellationToken);

            if (romaneos > 0)
                throw new ValidationException(
                    $"No se puede eliminar el tipificador porque tiene {romaneos} {(romaneos == 1 ? "romaneo" : "romaneos")} registrados. "
                    + "Si ya no trabaja en el palco, desactivelo en lugar de eliminarlo.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipificadorResponse();
        }
    }
}
