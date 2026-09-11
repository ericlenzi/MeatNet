using MediatR;
using Meat.Application.Puestos.Shared;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.UpdatePuesto
{
    public class UpdatePuestoHandler : IRequestHandler<UpdatePuestoRequest, UpdatePuestoResponse>
    {
        private readonly MeatContext context;

        public UpdatePuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdatePuestoResponse> Handle(UpdatePuestoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Puestos
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El puesto no existe.");

            await PuestoValidacion.ValidarAsync(
                this.context, request.EstablecimientoId, request.EspecieId, request.Nombre,
                request.TipoPuestoId, request.TipoMedicionId, cancellationToken);

            // Mover el puesto de establecimiento o de especie cambia el sentido de las listas de
            // matanza que ya lo tienen asignado, asi que no se permite una vez que se uso. El
            // nombre y el metodo de medicion si se pueden corregir.
            var cambioDeAlcance = entity.EstablecimientoId != request.EstablecimientoId
                || entity.EspecieId != request.EspecieId;

            if (cambioDeAlcance)
            {
                var enUso = await this.context.ListasMatanzas
                    .AnyAsync(lm => lm.PuestoId == entity.Id, cancellationToken);
                if (enUso)
                    throw new ValidationException(
                        "El puesto ya tiene listas de matanza asignadas, asi que no se puede cambiar de establecimiento ni de especie. "
                        + "Desactivelo y cree el puesto que corresponde.");
            }

            entity.Nombre = request.Nombre.Trim();
            entity.EstablecimientoId = request.EstablecimientoId;
            entity.EspecieId = request.EspecieId;
            entity.TipoPuestoId = request.TipoPuestoId;
            entity.TipoMedicionId = request.TipoMedicionId;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdatePuestoResponse();
        }
    }
}
