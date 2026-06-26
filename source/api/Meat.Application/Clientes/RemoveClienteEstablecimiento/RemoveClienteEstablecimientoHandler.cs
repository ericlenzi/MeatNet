using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.RemoveClienteEstablecimiento
{
    public class RemoveClienteEstablecimientoHandler : IRequestHandler<RemoveClienteEstablecimientoRequest, RemoveClienteEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public RemoveClienteEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<RemoveClienteEstablecimientoResponse> Handle(RemoveClienteEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var clienteEstablecimiento = await this.context.ClientesEstablecimientos
                .FirstOrDefaultAsync(ce => ce.Id == request.ClienteEstablecimientoId, cancellationToken);

            if (clienteEstablecimiento == null)
                throw new ValidationException("La asignacion no existe.");

            this.context.ClientesEstablecimientos.Remove(clienteEstablecimiento);
            await this.context.SaveChangesAsync(cancellationToken);

            return new RemoveClienteEstablecimientoResponse();
        }
    }
}
