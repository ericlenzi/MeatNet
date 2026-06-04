using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.UpdateCliente
{
    public class UpdateClienteHandler : IRequestHandler<UpdateClienteRequest, UpdateClienteResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateClienteHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateClienteResponse> Handle(UpdateClienteRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Clientes
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El cliente no existe");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateClienteResponse();
        }
    }
}
