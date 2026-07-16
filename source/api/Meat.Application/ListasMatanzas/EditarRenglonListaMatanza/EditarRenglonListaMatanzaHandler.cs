using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.EditarRenglonListaMatanza
{
    /// <summary>
    /// Edicion controlada de cantidad y/o secuencia de un renglon sobre una LM
    /// CONFIRMADA o EN_EJECUCION (R-10, R-11, R-12). Registra INCREMENTO/DECREMENTO
    /// y/o CAMBIO_SECUENCIA e incrementa Version por cada cambio.
    /// </summary>
    public class EditarRenglonListaMatanzaHandler : IRequestHandler<EditarRenglonListaMatanzaRequest, EditarRenglonListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public EditarRenglonListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<EditarRenglonListaMatanzaResponse> Handle(EditarRenglonListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada
                && entity.EstadoListaMatanzaId != EstadosListaMatanza.EnEjecucion)
                throw new ValidationException("Solo se pueden editar renglones de una lista Confirmada o En Ejecucion. En Borrador use la edicion normal.");

            var renglon = entity.Renglones.FirstOrDefault(r => r.Id == request.RenglonId);
            if (renglon == null)
                throw new ValidationException("El renglon no existe en la lista.");

            var cambiaCantidad = request.Cantidad != renglon.Cantidad;
            var cambiaSecuencia = request.Secuencia != renglon.Secuencia;
            if (!cambiaCantidad && !cambiaSecuencia)
                return new EditarRenglonListaMatanzaResponse();

            // R-12: renglon completamente faenado esta congelado
            if (renglon.CantidadFaenada >= renglon.Cantidad)
                throw new ValidationException("El renglon ya esta completamente faenado y no admite cambios.");

            // En Ejecucion solo se tocan renglones sin faena registrada (R-14)
            if (entity.EstadoListaMatanzaId == EstadosListaMatanza.EnEjecucion && renglon.CantidadFaenada > 0)
                throw new ValidationException("En ejecucion solo se pueden editar renglones sin faena registrada.");

            // R-07/R-11: la secuencia solo se cambia entre renglones no faenados
            if (cambiaSecuencia && renglon.CantidadFaenada > 0)
                throw new ValidationException("No se puede cambiar la secuencia de un renglon con faena registrada.");

            if (cambiaCantidad)
            {
                if (request.Cantidad <= 0)
                    throw new ValidationException("La cantidad debe ser mayor a cero.");

                // R-11: nunca por debajo de lo ya faenado
                if (request.Cantidad < renglon.CantidadFaenada)
                    throw new ValidationException($"La cantidad no puede ser menor a lo ya faenado ({renglon.CantidadFaenada}).");

                // Incremento: validar disponibilidad (R-05)
                if (request.Cantidad > renglon.Cantidad)
                {
                    var totalOtros = entity.Renglones
                        .Where(r => r.Id != renglon.Id && r.TropaId == renglon.TropaId
                            && r.AlmacenId == renglon.AlmacenId && r.TipoEspecieId == renglon.TipoEspecieId)
                        .Sum(r => r.Cantidad);
                    await ListaMatanzaValidacion.ValidateDisponibilidadAsync(
                        this.context, entity, renglon.TropaId, renglon.AlmacenId, renglon.TipoEspecieId,
                        totalOtros + request.Cantidad, cancellationToken);
                }

                entity.Version += 1;
                this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    Version = entity.Version,
                    Fecha = DateTime.Now,
                    UsuarioId = request.UsuarioId,
                    TipoMovimiento = request.Cantidad > renglon.Cantidad ? TiposMovimientoLM.Incremento : TiposMovimientoLM.Decremento,
                    TropaId = renglon.TropaId,
                    AlmacenId = renglon.AlmacenId,
                    CantidadAnterior = renglon.Cantidad,
                    CantidadNueva = request.Cantidad,
                    Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? "Cambio de cantidad del renglon." : request.Motivo
                });
                renglon.Cantidad = request.Cantidad;
            }

            if (cambiaSecuencia)
            {
                entity.Version += 1;
                this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
                {
                    Id = Guid.NewGuid(),
                    ListaMatanzaId = entity.Id,
                    Version = entity.Version,
                    Fecha = DateTime.Now,
                    UsuarioId = request.UsuarioId,
                    TipoMovimiento = TiposMovimientoLM.CambioSecuencia,
                    TropaId = renglon.TropaId,
                    AlmacenId = renglon.AlmacenId,
                    SecuenciaAnterior = renglon.Secuencia,
                    SecuenciaNueva = request.Secuencia,
                    Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? "Cambio de secuencia del renglon." : request.Motivo
                });
                renglon.Secuencia = request.Secuencia;
            }

            entity.FechaActualizacion = DateTime.Now;
            await this.context.SaveChangesAsync(cancellationToken);

            return new EditarRenglonListaMatanzaResponse();
        }
    }
}
