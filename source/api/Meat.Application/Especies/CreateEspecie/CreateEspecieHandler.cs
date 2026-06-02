using AutoMapper;
using MediatR;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Especies.CreateEspecie
{
    public class CreateEspecieHandler : IRequestHandler<CreateEspecieRequest, CreateEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateEspecieResponse> Handle(CreateEspecieRequest request, CancellationToken cancellationToken)
        {
            var especie = new Domain.Especies.Especie() { Activo = true };
            this.mapper.Map(request, especie);

            this.context.Especies.Add(especie);

            await this.context.SaveChangesAsync();

            return new CreateEspecieResponse()
            {
                Codigo = especie.Codigo,
            };
        }
    }
}
