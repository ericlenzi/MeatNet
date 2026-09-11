using MediatR;
using Meat.Application.Shared;
using Meat.Application.Tipificadores.Shared;
using Meat.Domain.Tipificadores;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.CreateTipificador
{
    public class CreateTipificadorHandler : IRequestHandler<CreateTipificadorRequest, CreateTipificadorResponse>
    {
        private readonly MeatContext context;

        public CreateTipificadorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateTipificadorResponse> Handle(CreateTipificadorRequest request, CancellationToken cancellationToken)
        {
            var matricula = (request.Matricula ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(matricula))
                throw new ValidationException("La matricula es requerida.");

            var matriculaEnUso = await this.context.Tipificadores
                .AnyAsync(t => t.Matricula == matricula, cancellationToken);
            if (matriculaEnUso)
                throw new ValidationException("Ya existe un tipificador con esa matricula.");

            await TipificadorValidacion.ValidarAsync(
                this.context, request.EstablecimientoId, request.EspecieId, request.Nombre, cancellationToken);

            if (request.PorDefecto)
                await TipificadorValidacion.DestildarOtrosPorDefectoAsync(
                    this.context, request.EstablecimientoId, request.EspecieId, null, cancellationToken);

            var entity = TipificadorFactory.Create();
            entity.Nombre = request.Nombre.Trim();
            entity.Matricula = matricula;
            entity.EstablecimientoId = request.EstablecimientoId;
            entity.EspecieId = request.EspecieId;
            entity.PorDefecto = request.PorDefecto;

            this.context.Tipificadores.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipificadorResponse { Id = entity.Id };
        }
    }
}
