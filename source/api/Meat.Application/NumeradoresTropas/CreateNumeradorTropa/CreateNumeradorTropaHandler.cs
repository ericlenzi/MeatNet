using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.NumeradoresTropas.CreateNumeradorTropa
{
    public class CreateNumeradorTropaHandler : IRequestHandler<CreateNumeradorTropaRequest, CreateNumeradorTropaResponse>
    {
        private readonly MeatContext context;

        public CreateNumeradorTropaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateNumeradorTropaResponse> Handle(CreateNumeradorTropaRequest request, CancellationToken cancellationToken)
        {
            var clienteEstablecimiento = await this.context.ClientesEstablecimientos
                .FirstOrDefaultAsync(ce => ce.Id == request.ClienteEstablecimientoId, cancellationToken);
            if (clienteEstablecimiento == null)
                throw new ValidationException("El ClienteEstablecimiento no existe.");

            var especie = await this.context.Especies
                .FirstOrDefaultAsync(e => e.Codigo == request.EspecieCodigo, cancellationToken);
            if (especie == null)
                throw new ValidationException("La especie no existe.");

            var exists = await this.context.NumeradoresTropas
                .AnyAsync(nt => nt.ClienteEstablecimientoId == request.ClienteEstablecimientoId
                             && nt.EspecieCodigo == request.EspecieCodigo, cancellationToken);
            if (exists)
                throw new ValidationException("Ya existe un numerador para esa combinacion de ClienteEstablecimiento y Especie.");

            var entity = new Domain.NumeradoresTropas.NumeradorTropa
            {
                Id = Guid.NewGuid(),
                ClienteEstablecimientoId = request.ClienteEstablecimientoId,
                EspecieCodigo = request.EspecieCodigo,
                UltimoNumeroTropa = request.UltimoNumeroTropa
            };

            this.context.NumeradoresTropas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateNumeradorTropaResponse { Id = entity.Id };
        }
    }
}
