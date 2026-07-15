using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.QuitarRenglonListaMatanza
{
    /// <summary>
    /// Baja controlada de un renglon sobre una LM CONFIRMADA, solo si no tiene
    /// faena registrada (R-10). Registra movimiento BAJA_TROPA e incrementa Version.
    /// </summary>
    public class QuitarRenglonListaMatanzaHandler : IRequestHandler<QuitarRenglonListaMatanzaRequest, QuitarRenglonListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public QuitarRenglonListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<QuitarRenglonListaMatanzaResponse> Handle(QuitarRenglonListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada)
                throw new ValidationException("Solo se puede quitar un renglon de una lista Confirmada.");

            var renglon = entity.Renglones.FirstOrDefault(r => r.Id == request.RenglonId);
            if (renglon == null)
                throw new ValidationException("El renglon no existe en la lista.");

            if (renglon.CantidadFaenada > 0)
                throw new ValidationException("No se puede quitar un renglon con faena registrada.");

            if (entity.Renglones.Count <= 1)
                throw new ValidationException("La lista confirmada debe conservar al menos un renglon. Para dejarla sin renglones, vuelva a Borrador o anulela.");

            entity.Version += 1;
            entity.FechaActualizacion = DateTime.Now;
            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = DateTime.Now,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.BajaTropa,
                TropaId = renglon.TropaId,
                AlmacenId = renglon.AlmacenId,
                CantidadAnterior = renglon.Cantidad,
                SecuenciaAnterior = renglon.Secuencia,
                Motivo = "Baja de tropa en lista confirmada."
            });

            this.context.ListasMatanzasDetalles.Remove(renglon);   // soft delete
            await this.context.SaveChangesAsync(cancellationToken);

            return new QuitarRenglonListaMatanzaResponse();
        }
    }
}
