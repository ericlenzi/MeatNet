using AutoMapper;
using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Categorias.CreateCategoria
{
    public class CreateCategoriaHandler : IRequestHandler<CreateCategoriaRequest, CreateCategoriaResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateCategoriaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateCategoriaResponse> Handle(CreateCategoriaRequest request, CancellationToken cancellationToken)
        {
            if (this.context.Categorias.Any(x => x.Id == request.Id))
                throw new ValidationException("Ya existe una categoria con ese codigo.");

            var entity = new Domain.Categorias.Categoria();
            this.mapper.Map(request, entity);
            entity.Activo = true;
            entity.FechaActualizacion = DateTime.Now;

            this.context.Categorias.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateCategoriaResponse { Id = entity.Id };
        }
    }
}
