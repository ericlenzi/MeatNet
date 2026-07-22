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
    /// Baja controlada de un renglon sobre una LM CONFIRMADA o EN_EJECUCION, solo si no
    /// tiene faena registrada (R-10, R-14): un renglon sin romaneos no consumio stock, y
    /// en ejecucion es la forma de corregir una faena de emergencia cargada por error.
    /// Registra movimiento BAJA_TROPA e incrementa Version.
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

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada
                && entity.EstadoListaMatanzaId != EstadosListaMatanza.EnEjecucion)
                throw new ValidationException("Solo se puede quitar un renglon de una lista Confirmada o En Ejecucion.");

            var renglon = entity.Renglones.FirstOrDefault(r => r.Id == request.RenglonId);
            if (renglon == null)
                throw new ValidationException("El renglon no existe en la lista.");

            if (renglon.CantidadFaenada > 0)
                throw new ValidationException("No se puede quitar un renglon con faena registrada.");

            // CantidadFaenada = 0 no alcanza: si el renglon tuvo romaneos y se anularon, la baja
            // los dejaria fuera de la grilla de la jornada (que joinea el renglon) y sin rastro.
            var tuvoRomaneos = await this.context.Romaneos
                .AnyAsync(r => r.ListaMatanzaDetalleId == renglon.Id, cancellationToken);
            if (tuvoRomaneos)
                throw new ValidationException("No se puede quitar un renglon con romaneos registrados, aunque esten anulados.");

            if (entity.Renglones.Count <= 1)
                throw new ValidationException(entity.EstadoListaMatanzaId == EstadosListaMatanza.EnEjecucion
                    ? "La lista debe conservar al menos un renglon. Para cerrar la jornada, finalicela."
                    : "La lista confirmada debe conservar al menos un renglon. Para dejarla sin renglones, vuelva a Borrador o anulela.");

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
                Motivo = entity.EstadoListaMatanzaId == EstadosListaMatanza.EnEjecucion
                    ? "Baja de tropa sin faena en lista en ejecucion."
                    : "Baja de tropa en lista confirmada."
            });

            this.context.ListasMatanzasDetalles.Remove(renglon);   // soft delete
            await this.context.SaveChangesAsync(cancellationToken);

            return new QuitarRenglonListaMatanzaResponse();
        }
    }
}
