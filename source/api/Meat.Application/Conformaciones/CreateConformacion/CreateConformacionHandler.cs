using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Conformaciones.CreateConformacion
{
    public class CreateConformacionHandler : IRequestHandler<CreateConformacionRequest, CreateConformacionResponse>
    {
        private readonly MeatContext context;

        public CreateConformacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateConformacionResponse> Handle(CreateConformacionRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.Conformaciones.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe la conformacion con ese codigo.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var entity = new Domain.Conformaciones.Conformacion
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                EspecieId = request.EspecieId,
                Orden = request.Orden,
                Activo = true,
            };

            this.context.Conformaciones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateConformacionResponse { Codigo = entity.Codigo };
        }
    }
}
