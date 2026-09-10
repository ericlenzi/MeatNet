using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Denticiones.CreateDenticion
{
    public class CreateDenticionHandler : IRequestHandler<CreateDenticionRequest, CreateDenticionResponse>
    {
        private readonly MeatContext context;

        public CreateDenticionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateDenticionResponse> Handle(CreateDenticionRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.Denticiones.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe la denticion con ese codigo.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var entity = new Meat.Domain.Denticiones.Denticion
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                EspecieId = request.EspecieId,
                Orden = request.Orden,
                Activo = true,
            };

            this.context.Denticiones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateDenticionResponse { Codigo = entity.Codigo };
        }
    }
}
