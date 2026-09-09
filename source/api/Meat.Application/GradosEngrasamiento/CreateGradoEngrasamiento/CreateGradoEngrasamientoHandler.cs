using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.GradosEngrasamiento.CreateGradoEngrasamiento
{
    public class CreateGradoEngrasamientoHandler : IRequestHandler<CreateGradoEngrasamientoRequest, CreateGradoEngrasamientoResponse>
    {
        private readonly MeatContext context;

        public CreateGradoEngrasamientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateGradoEngrasamientoResponse> Handle(CreateGradoEngrasamientoRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.GradosEngrasamiento.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe el grado de engrasamiento con ese codigo.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var entity = new Domain.GradosEngrasamiento.GradoEngrasamiento
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                EspecieId = request.EspecieId,
                Orden = request.Orden,
                Activo = true,
            };

            this.context.GradosEngrasamiento.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateGradoEngrasamientoResponse { Codigo = entity.Codigo };
        }
    }
}
