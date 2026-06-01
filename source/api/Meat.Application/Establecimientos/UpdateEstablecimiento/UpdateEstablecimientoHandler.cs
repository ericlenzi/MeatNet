using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoHandler : IRequestHandler<UpdateEstablecimientoRequest, UpdateEstablecimientoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateEstablecimientoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateEstablecimientoResponse> Handle(UpdateEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Establecimientos
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El establecimiento no existe");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateEstablecimientoResponse();
        }
    }
}
