using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Categorias.UpdateCategoria
{
    public class UpdateCategoriaHandler : IRequestHandler<UpdateCategoriaRequest, UpdateCategoriaResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateCategoriaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateCategoriaResponse> Handle(UpdateCategoriaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Categorias
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("La categoria no existe.");

            this.mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateCategoriaResponse();
        }
    }
}
