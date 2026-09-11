using MediatR;
using Meat.Application.Puestos.Shared;
using Meat.Application.Shared;
using Meat.Domain.Puestos;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.CreatePuesto
{
    public class CreatePuestoHandler : IRequestHandler<CreatePuestoRequest, CreatePuestoResponse>
    {
        private readonly MeatContext context;

        public CreatePuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreatePuestoResponse> Handle(CreatePuestoRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.CodigoPuesto ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo del puesto es requerido.");

            var codigoEnUso = await this.context.Puestos
                .AnyAsync(p => p.CodigoPuesto == codigo, cancellationToken);
            if (codigoEnUso)
                throw new ValidationException("Ya existe un puesto con ese codigo.");

            await PuestoValidacion.ValidarAsync(
                this.context, request.EstablecimientoId, request.EspecieId, request.Nombre,
                request.TipoPuestoId, request.TipoMedicionId, cancellationToken);

            var entity = PuestoFactory.Create();
            entity.CodigoPuesto = codigo;
            entity.Nombre = request.Nombre.Trim();
            entity.EstablecimientoId = request.EstablecimientoId;
            entity.EspecieId = request.EspecieId;
            entity.TipoPuestoId = request.TipoPuestoId;
            entity.TipoMedicionId = request.TipoMedicionId;

            this.context.Puestos.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreatePuestoResponse { Id = entity.Id };
        }
    }
}
