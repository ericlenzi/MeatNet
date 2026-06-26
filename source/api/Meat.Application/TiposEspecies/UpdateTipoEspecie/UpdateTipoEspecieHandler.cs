using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEspecies.UpdateTipoEspecie
{
    public class UpdateTipoEspecieHandler : IRequestHandler<UpdateTipoEspecieRequest, UpdateTipoEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateTipoEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateTipoEspecieResponse> Handle(UpdateTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposEspecies
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de especie no existe.");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipoEspecieResponse();
        }
    }
}
