using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.CreateEmpresa
{
    public class CreateEmpresaHandler : IRequestHandler<CreateEmpresaRequest, CreateEmpresaResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateEmpresaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateEmpresaResponse> Handle(CreateEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = Domain.Empresas.EmpresaFactory.Create();
            this.mapper.Map(request, empresa);

            this.context.Empresas.Add(empresa);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEmpresaResponse()
            {
                Id = empresa.Id
            };
        }
    }
}
