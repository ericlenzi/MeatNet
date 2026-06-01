using AutoMapper;
using MediatR;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoHandler : IRequestHandler<CreateEstablecimientoRequest, CreateEstablecimientoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateEstablecimientoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateEstablecimientoResponse> Handle(CreateEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var entity = new Domain.Establecimientos.Establecimiento
            {
                Id = Guid.NewGuid(),
                Activo = true,
                FechaActualizacion = DateTime.Now
            };
            this.mapper.Map(request, entity);

            this.context.Establecimientos.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEstablecimientoResponse { Id = entity.Id };
        }
    }
}
