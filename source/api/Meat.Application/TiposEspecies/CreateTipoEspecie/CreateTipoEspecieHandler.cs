using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
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
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es obligatorio.");

            if (await this.context.TiposEspecies.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe un tipo de especie con ese codigo.");

            if (!await this.context.Especies.AnyAsync(x => x.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            if (!string.IsNullOrEmpty(request.TipoSexoId)
                && !await this.context.TiposSexos.AnyAsync(x => x.Codigo == request.TipoSexoId, cancellationToken))
                throw new ValidationException("El tipo de sexo indicado no existe.");

            var entity = new Domain.TiposEspecies.TipoEspecie { Activo = true };
            this.mapper.Map(request, entity);
            entity.Codigo = codigo;

            this.context.TiposEspecies.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipoEspecieResponse { Codigo = entity.Codigo };
        }
    }
}
