using MediatR;
using Meat.Application.Shared;
using Meat.Application.Tipificadores.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.UpdateTipificador
{
    public class UpdateTipificadorHandler : IRequestHandler<UpdateTipificadorRequest, UpdateTipificadorResponse>
    {
        private readonly MeatContext context;

        public UpdateTipificadorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateTipificadorResponse> Handle(UpdateTipificadorRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Tipificadores
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El tipificador no existe.");

            var matricula = (request.Matricula ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(matricula))
                throw new ValidationException("La matricula es requerida.");

            var matriculaEnUso = await this.context.Tipificadores
                .AnyAsync(t => t.Id != entity.Id && t.Matricula == matricula, cancellationToken);
            if (matriculaEnUso)
                throw new ValidationException("Ya existe un tipificador con esa matricula.");

            await TipificadorValidacion.ValidarAsync(
                this.context, request.EstablecimientoId, request.EspecieId, request.Nombre, cancellationToken);

            if (request.PorDefecto)
                await TipificadorValidacion.DestildarOtrosPorDefectoAsync(
                    this.context, request.EstablecimientoId, request.EspecieId, entity.Id, cancellationToken);

            entity.Nombre = request.Nombre.Trim();
            entity.Matricula = matricula;
            entity.EstablecimientoId = request.EstablecimientoId;
            entity.EspecieId = request.EspecieId;
            entity.PorDefecto = request.PorDefecto;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipificadorResponse();
        }
    }
}
