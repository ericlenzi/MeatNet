using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Puestos.UpdatePuesto
{

    public class UpdatePuestoHandler : IRequestHandler<UpdatePuestoRequest, UpdatePuestoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdatePuestoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdatePuestoResponse> Handle(UpdatePuestoRequest request, CancellationToken cancellationToken)
        {
            var puesto = await this.context.Puestos.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (puesto == null)
            {
                throw new ValidationException("El puesto de trabajo no existe");
            }

            this.mapper.Map(request, puesto);

            await this.context.SaveChangesAsync();

            return new UpdatePuestoResponse();
        }
    }
}
