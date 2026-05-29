using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.GetPuesto
{
    public class GetPuestoHandler : IRequestHandler<GetPuestoRequest, GetPuestoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetPuestoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetPuestoResponse> Handle(GetPuestoRequest request, CancellationToken cancellationToken)
        {
            var puesto = await this.context.Puestos.FirstOrDefaultAsync(p => p.Id == request.Id);

            return this.mapper.Map<GetPuestoResponse>(puesto);
        }
    }
}