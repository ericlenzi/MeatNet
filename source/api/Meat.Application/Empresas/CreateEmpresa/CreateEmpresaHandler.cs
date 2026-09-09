using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Empresas.Seed;
using Meat.Application.Empresas.Shared;
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
            if (string.IsNullOrWhiteSpace(request.Id))
                throw new ValidationException("Debe indicar el codigo de la empresa.");

            var yaExiste = await this.context.Empresas.AnyAsync(x => x.Id == request.Id, cancellationToken);
            if (yaExiste)
                throw new ValidationException("Ya existe una empresa con ese codigo.");

            EmpresaValidacion.ValidarColor(request.Color);
            EmpresaValidacion.ValidarLogo(request.Logo);

            var empresa = Domain.Empresas.EmpresaFactory.Create(request.Id);
            this.mapper.Map(request, empresa);

            this.context.Empresas.Add(empresa);

            // Una empresa vacia no se puede usar: sin sucursal nadie puede entrar, y sin
            // master data no se puede planificar una faena. Nace operable o no sirve.
            new EmpresaSeeder(this.context).Sembrar(empresa.Id, empresa.Nombre);

            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEmpresaResponse()
            {
                Id = empresa.Id
            };
        }
    }
}
