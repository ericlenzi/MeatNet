using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

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
            var puesto = await this.context.Puestos.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (puesto == null)
            {
                throw new ValidationException("El puesto no existe");
            }

            this.context.Remove(puesto);

            await this.context.SaveChangesAsync();

            return new DeletePuestoResponse();
        }
    }
}
