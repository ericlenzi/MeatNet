using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.CreateListaMatanza
{
    public class CreateListaMatanzaHandler : IRequestHandler<CreateListaMatanzaRequest, CreateListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public CreateListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateListaMatanzaResponse> Handle(CreateListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            // Establecimiento activo dentro de la empresa
            var establecimiento = await this.context.Establecimientos
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId
                    && e.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (establecimiento == null)
                throw new ValidationException("El establecimiento activo no es valido.");

            // La especie debe estar habilitada para el establecimiento
            if (string.IsNullOrEmpty(request.EspecieId))
                throw new ValidationException("Debe indicar la especie de la lista de matanza.");
            var especieHabilitada = await this.context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EstablecimientoId == establecimiento.Id
                    && ee.EspecieId == request.EspecieId, cancellationToken);
            if (!especieHabilitada)
                throw new ValidationException("La especie no esta habilitada para el establecimiento.");

            // Puesto (palco de faena) opcional, debe pertenecer al establecimiento
            if (request.PuestoId.HasValue)
            {
                var puestoValido = await this.context.Puestos
                    .AnyAsync(p => p.Id == request.PuestoId.Value
                        && p.EstablecimientoId == establecimiento.Id, cancellationToken);
                if (!puestoValido)
                    throw new ValidationException("El puesto indicado no pertenece al establecimiento.");
            }

            // R-01: una LM activa (no cancelada) por Establecimiento + Fecha + Especie
            var fecha = request.Fecha.Date;
            var yaExiste = await this.context.ListasMatanzas
                .AnyAsync(lm => lm.EstablecimientoId == establecimiento.Id
                    && lm.Fecha == fecha
                    && lm.EspecieId == request.EspecieId
                    && lm.EstadoListaMatanzaId != EstadosListaMatanza.Anulada, cancellationToken);
            if (yaExiste)
                throw new ValidationException("Ya existe una lista de matanza para ese establecimiento, fecha y especie.");

            // Validar renglones (cantidad y tope En Pie por tropa/corral)
            await ListaMatanzaValidacion.ValidateRenglonesAsync(
                this.context, establecimiento.Id, request.EspecieId, request.Renglones, cancellationToken);
            // Validar destinos (camaras) informados; en Borrador el destino es opcional
            await ListaMatanzaValidacion.ValidateDestinosAsync(
                this.context, establecimiento.Id, request.Renglones, cancellationToken);

            // R-02: numero de lista correlativo por establecimiento (MAX + 1)
            var maxNumero = await this.context.ListasMatanzas
                .Where(lm => lm.EstablecimientoId == establecimiento.Id)
                .Select(lm => (long?)lm.NumeroLista)
                .MaxAsync(cancellationToken) ?? 0;

            var entity = ListaMatanzaFactory.Create();
            entity.EstablecimientoId = establecimiento.Id;
            entity.EspecieId = request.EspecieId;
            entity.PuestoId = request.PuestoId;
            entity.Fecha = fecha;
            entity.NumeroLista = maxNumero + 1;
            entity.EstadoListaMatanzaId = EstadosListaMatanza.Borrador;

            entity.Renglones = request.Renglones
                .Select(r => new ListaMatanzaDetalle
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    TropaId = r.TropaId,
                    AlmacenId = r.AlmacenId,
                    AlmacenDestinoId = r.AlmacenDestinoId,
                    TipoEspecieId = r.TipoEspecieId,
                    Secuencia = r.Secuencia,
                    Cantidad = r.Cantidad,
                    CantidadFaenada = 0
                })
                .ToList();

            this.context.ListasMatanzas.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateListaMatanzaResponse
            {
                Id = entity.Id,
                NumeroLista = entity.NumeroLista
            };
        }
    }
}
