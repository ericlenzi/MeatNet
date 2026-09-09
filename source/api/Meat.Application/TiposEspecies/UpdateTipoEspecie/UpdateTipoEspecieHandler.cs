using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
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
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de especie no existe.");

            if (!await this.context.Especies.AnyAsync(x => x.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            if (!string.IsNullOrEmpty(request.TipoSexoId)
                && !await this.context.TiposSexos.AnyAsync(x => x.Codigo == request.TipoSexoId, cancellationToken))
                throw new ValidationException("El tipo de sexo indicado no existe.");

            this.mapper.Map(request, entity);

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipoEspecieResponse();
        }
    }
}
