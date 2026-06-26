using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.AddClienteEstablecimiento
{
    public class AddClienteEstablecimientoHandler : IRequestHandler<AddClienteEstablecimientoRequest, AddClienteEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public AddClienteEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AddClienteEstablecimientoResponse> Handle(AddClienteEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var establecimiento = await this.context.Establecimientos
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId, cancellationToken);

            if (establecimiento == null)
                throw new ValidationException("El establecimiento no existe.");

            var exists = await this.context.ClientesEstablecimientos
                .AnyAsync(ce => ce.ClienteId == request.ClienteId && ce.EstablecimientoId == request.EstablecimientoId, cancellationToken);

            if (exists)
                throw new ValidationException("El establecimiento ya esta asignado al cliente.");

            var clienteEstablecimiento = new Domain.ClientesEstablecimientos.ClienteEstablecimiento
            {
                Id = Guid.NewGuid(),
                ClienteId = request.ClienteId,
                EstablecimientoId = request.EstablecimientoId,
                CodigoRenspa = request.CodigoRenspa,
                NumeroCUIG = request.NumeroCUIG
            };

            this.context.ClientesEstablecimientos.Add(clienteEstablecimiento);
            await this.context.SaveChangesAsync(cancellationToken);

            return new AddClienteEstablecimientoResponse { Id = clienteEstablecimiento.Id };
        }
    }
}
