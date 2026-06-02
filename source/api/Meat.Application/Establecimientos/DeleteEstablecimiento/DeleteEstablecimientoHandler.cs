using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.DeleteEstablecimiento
{
    public class DeleteEstablecimientoHandler : IRequestHandler<DeleteEstablecimientoRequest, DeleteEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public DeleteEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEstablecimientoResponse> Handle(DeleteEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Establecimientos
                .Include(x => x.Sucursal).ThenInclude(s => s.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Sucursal.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El establecimiento no existe");

            this.context.Establecimientos.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEstablecimientoResponse();
        }
    }
}
