using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
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
            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var entity = Domain.Clientes.ClienteFactory.Create();
            this.mapper.Map(request, entity);
            entity.EmpresaId = empresa.Id;

            this.context.Clientes.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateClienteResponse { Id = entity.Id };
        }
    }
}
