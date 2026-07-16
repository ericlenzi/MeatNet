using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.UpdateListaMatanza
{
    public class UpdateListaMatanzaHandler : IRequestHandler<UpdateListaMatanzaRequest, UpdateListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public UpdateListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateListaMatanzaResponse> Handle(UpdateListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            // R-08: la edicion libre solo aplica en Borrador
            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Borrador)
                throw new ValidationException("Solo se puede modificar libremente una lista en Borrador.");

            if (string.IsNullOrEmpty(request.EspecieId))
                throw new ValidationException("Debe indicar la especie de la lista de matanza.");
            var especieHabilitada = await this.context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EstablecimientoId == entity.EstablecimientoId
                    && ee.EspecieId == request.EspecieId, cancellationToken);
            if (!especieHabilitada)
                throw new ValidationException("La especie no esta habilitada para el establecimiento.");

            // R-01: unicidad (Establecimiento + Fecha + Especie), ignorando canceladas y la propia LM
            var fecha = request.Fecha.Date;
            var yaExiste = await this.context.ListasMatanzas
                .AnyAsync(lm => lm.Id != entity.Id
                    && lm.EstablecimientoId == entity.EstablecimientoId
                    && lm.Fecha == fecha
                    && lm.EspecieId == request.EspecieId
                    && lm.EstadoListaMatanzaId != EstadosListaMatanza.Anulada, cancellationToken);
            if (yaExiste)
                throw new ValidationException("Ya existe una lista de matanza para ese establecimiento, fecha y especie.");

            await ListaMatanzaValidacion.ValidateRenglonesAsync(
                this.context, entity.EstablecimientoId, request.EspecieId, request.Renglones, cancellationToken);

            entity.EspecieId = request.EspecieId;
            entity.Fecha = fecha;
            entity.FechaActualizacion = DateTime.Now;

            // Reemplazo total de renglones (dividir/fusionar/resecuenciar se resuelven en el cliente en Borrador)
            this.context.ListasMatanzasDetalles.RemoveRange(entity.Renglones);
            var nuevos = request.Renglones
                .Select(r => new ListaMatanzaDetalle
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    TropaId = r.TropaId,
                    AlmacenId = r.AlmacenId,
                    TipoEspecieId = r.TipoEspecieId,
                    Secuencia = r.Secuencia,
                    Cantidad = r.Cantidad,
                    CantidadFaenada = 0
                })
                .ToList();
            this.context.ListasMatanzasDetalles.AddRange(nuevos);

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateListaMatanzaResponse();
        }
    }
}
