using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.AgregarRenglonListaMatanza
{
    /// <summary>
    /// Alta controlada de un renglon sobre una LM CONFIRMADA (R-10).
    /// Registra movimiento ALTA_TROPA e incrementa Version.
    /// </summary>
    public class AgregarRenglonListaMatanzaHandler : IRequestHandler<AgregarRenglonListaMatanzaRequest, AgregarRenglonListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public AgregarRenglonListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AgregarRenglonListaMatanzaResponse> Handle(AgregarRenglonListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada)
                throw new ValidationException("Solo se puede agregar un renglon a una lista Confirmada. En Ejecucion use faena de emergencia.");

            if (request.Cantidad <= 0)
                throw new ValidationException("La cantidad debe ser mayor a cero.");

            var totalActual = entity.Renglones
                .Where(r => r.TropaId == request.TropaId && r.AlmacenId == request.AlmacenId)
                .Sum(r => r.Cantidad);
            await ListaMatanzaValidacion.ValidateDisponibilidadAsync(
                this.context, entity, request.TropaId, request.AlmacenId,
                totalActual + request.Cantidad, cancellationToken);

            var secuencia = request.Secuencia
                ?? (entity.Renglones.Any() ? entity.Renglones.Max(r => r.Secuencia) + 10 : 10);

            var renglon = new ListaMatanzaDetalle
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                TropaId = request.TropaId,
                AlmacenId = request.AlmacenId,
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
                TipoMovimiento = TiposMovimientoLM.AltaTropa,
                TropaId = request.TropaId,
                AlmacenId = request.AlmacenId,
                CantidadNueva = request.Cantidad,
                SecuenciaNueva = secuencia,
                Motivo = string.IsNullOrWhiteSpace(request.Motivo) ? "Alta de tropa en lista confirmada." : request.Motivo
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return new AgregarRenglonListaMatanzaResponse { RenglonId = renglon.Id };
        }
    }
}
