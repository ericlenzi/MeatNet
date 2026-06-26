using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.CreateCliente
{
    public class CreateClienteHandler : IRequestHandler<CreateClienteRequest, CreateClienteResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateClienteHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateClienteResponse> Handle(CreateClienteRequest request, CancellationToken cancellationToken)
        {
            var existe = await this.context.Clientes
                .AnyAsync(c => c.CodigoCliente == request.CodigoCliente, cancellationToken);
            if (existe)
                throw new ValidationException("Ya existe un cliente con ese codigo.");

            var entity = Domain.Clientes.ClienteFactory.Create();
            this.mapper.Map(request, entity);

            this.context.Clientes.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateClienteResponse { Id = entity.Id };
        }
    }
}
