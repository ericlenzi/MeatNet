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
        private const string TipoEmpresaPropia = "PRP";
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateEmpresaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateEmpresaResponse> Handle(CreateEmpresaRequest request, CancellationToken cancellationToken)
        {
            if (request.TipoEmpresaId == TipoEmpresaPropia)
            {
                var existePropia = await this.context.Empresas
                    .AnyAsync(e => e.TipoEmpresaId == TipoEmpresaPropia, cancellationToken);

                if (existePropia)
                    throw new ValidationException("Ya existe una empresa propia. Solo puede haber una empresa con tipo PRP.");
            }

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
