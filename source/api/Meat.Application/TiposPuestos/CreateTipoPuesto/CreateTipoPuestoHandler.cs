using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposPuestos.CreateTipoPuesto
{
    public class CreateTipoPuestoHandler : IRequestHandler<CreateTipoPuestoRequest, CreateTipoPuestoResponse>
    {
        private readonly MeatContext context;

        public CreateTipoPuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateTipoPuestoResponse> Handle(CreateTipoPuestoRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.TiposPuestos.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe un tipo de puesto con ese codigo.");

            var entity = new Domain.TiposPuestos.TipoPuesto
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                Activo = true,
            };

            this.context.TiposPuestos.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipoPuestoResponse { Codigo = entity.Codigo };
        }
    }
}
