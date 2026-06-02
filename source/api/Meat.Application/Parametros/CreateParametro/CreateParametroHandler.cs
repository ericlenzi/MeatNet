using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroHandler : IRequestHandler<CreateParametroRequest, CreateParametroResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateParametroHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateParametroResponse> Handle(CreateParametroRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas.FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var parametro = new Domain.Parametros.Parametro() { Activo = true, EmpresaId = empresa.Id };
            this.mapper.Map(request, parametro);

            this.context.Parametros.Add(parametro);

            await this.context.SaveChangesAsync();

            return new CreateParametroResponse()
            {
                Codigo = parametro.Codigo,
            };
        }
    }
}
