using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMediciones.CreateTipoMedicion
{
    public class CreateTipoMedicionHandler : IRequestHandler<CreateTipoMedicionRequest, CreateTipoMedicionResponse>
    {
        private readonly MeatContext context;

        public CreateTipoMedicionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateTipoMedicionResponse> Handle(CreateTipoMedicionRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.TiposMediciones.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe un tipo de medicion con ese codigo.");

            var entity = new Domain.TiposMediciones.TipoMedicion
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                Activo = true,
            };

            this.context.TiposMediciones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipoMedicionResponse { Codigo = entity.Codigo };
        }
    }
}
