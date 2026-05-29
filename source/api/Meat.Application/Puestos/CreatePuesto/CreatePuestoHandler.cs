using AutoMapper;
using MediatR;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Meat.Application.Shared;

namespace Meat.Application.Puestos.CreatePuesto
{
    public class CreatePuestoHandler : IRequestHandler<CreatePuestoRequest, CreatePuestoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreatePuestoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreatePuestoResponse> Handle(CreatePuestoRequest request, CancellationToken cancellationToken)
        {
            var puesto = Domain.Puestos.PuestoFactory.Create();
            this.mapper.Map(request, puesto);

            this.context.Puestos.Add(puesto);

            await this.context.SaveChangesAsync();

            return new CreatePuestoResponse()
            {
                Id = puesto.Id,
            };
        }
    }
}
