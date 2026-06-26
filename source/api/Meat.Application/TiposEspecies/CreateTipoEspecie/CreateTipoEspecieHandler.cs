using AutoMapper;
using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEspecies.CreateTipoEspecie
{
    public class CreateTipoEspecieHandler : IRequestHandler<CreateTipoEspecieRequest, CreateTipoEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateTipoEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateTipoEspecieResponse> Handle(CreateTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            if (this.context.TiposEspecies.Any(x => x.Id == request.Id))
                throw new ValidationException("Ya existe un tipo de especie con ese codigo.");

            var entity = new Domain.TiposEspecies.TipoEspecie();
            this.mapper.Map(request, entity);
            entity.Activo = true;
            entity.FechaActualizacion = DateTime.Now;

            this.context.TiposEspecies.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipoEspecieResponse { Id = entity.Id };
        }
    }
}
