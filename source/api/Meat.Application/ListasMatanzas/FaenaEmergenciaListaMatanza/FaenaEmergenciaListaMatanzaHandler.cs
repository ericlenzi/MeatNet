using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.FaenaEmergenciaListaMatanza
{
    /// <summary>
    /// R-14: faena de emergencia sobre una LM EN_EJECUCION. Agrega un renglon nuevo
    /// anexado al final de la secuencia, desde una tropa existente En Pie.
    /// Registra movimiento FAENA_EMERGENCIA e incrementa Version.
    /// </summary>
    public class FaenaEmergenciaListaMatanzaHandler : IRequestHandler<FaenaEmergenciaListaMatanzaRequest, FaenaEmergenciaListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public FaenaEmergenciaListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<FaenaEmergenciaListaMatanzaResponse> Handle(FaenaEmergenciaListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.EnEjecucion)
                throw new ValidationException("La faena de emergencia solo aplica a una lista En Ejecucion.");

            if (request.Cantidad <= 0)
                throw new ValidationException("La cantidad debe ser mayor a cero.");

            if (string.IsNullOrEmpty(request.TipoEspecieId))
                throw new ValidationException("Debe indicar la categoria (tipo de especie).");

            var totalActual = entity.Renglones
                .Where(r => r.TropaId == request.TropaId && r.AlmacenId == request.AlmacenId && r.TipoEspecieId == request.TipoEspecieId)
                .Sum(r => r.Cantidad);
            await ListaMatanzaValidacion.ValidateDisponibilidadAsync(
                this.context, entity, request.TropaId, request.AlmacenId, request.TipoEspecieId,
                totalActual + request.Cantidad, cancellationToken);

            // R-14: siempre anexada al final de la secuencia
            var secuencia = entity.Renglones.Any() ? entity.Renglones.Max(r => r.Secuencia) + 10 : 10;

            var renglon = new ListaMatanzaDetalle
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                TropaId = request.TropaId,
                AlmacenId = request.AlmacenId,
                TipoEspecieId = request.TipoEspecieId,
                Secuencia = secuencia,
                Cantidad = request.Cantidad,
                CantidadFaenada = 0
            };
            this.context.ListasMatanzasDetalles.Add(renglon);

            entity.Version += 1;
            entity.FechaActualizacion = DateTime.Now;
            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = DateTime.Now,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.FaenaEmergencia,
                TropaId = request.TropaId,
                AlmacenId = request.AlmacenId,
                CantidadNueva = request.Cantidad,
                SecuenciaNueva = secuencia,
                Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? "Faena de emergencia." : request.Motivo
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return new FaenaEmergenciaListaMatanzaResponse
            {
                RenglonId = renglon.Id,
                Secuencia = secuencia
            };
        }
    }
}
