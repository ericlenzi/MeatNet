using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Application.Shared;
using Meat.Application.Tropas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.AnularRomaneo
{
    /// <summary>
    /// Anula un romaneo (correccion). Revierte el consumo: baja CantidadFaenada, resta Puntos,
    /// devuelve el animal a En Pie (tropa a RECEPCIONADA) y audita la reversa en la trazabilidad.
    /// El registro no se borra (Anulado = true).
    /// </summary>
    public class AnularRomaneoHandler : IRequestHandler<AnularRomaneoRequest, AnularRomaneoResponse>
    {
        private readonly MeatContext context;

        public AnularRomaneoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AnularRomaneoResponse> Handle(AnularRomaneoRequest request, CancellationToken cancellationToken)
        {
            var romaneo = await this.context.Romaneos
                .Include(r => r.Piezas)
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (romaneo == null)
                throw new ValidationException("El romaneo no existe.");

            // Verificar empresa via la LM
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(x => x.Id == romaneo.ListaMatanzaId
                    && x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (lm == null)
                throw new ValidationException("El romaneo no pertenece a la empresa.");

            if (romaneo.Anulado)
                throw new ValidationException("El romaneo ya esta anulado.");

            romaneo.Anulado = true;

            // Revertir consumo del renglon
            var renglon = await this.context.ListasMatanzasDetalles
                .FirstOrDefaultAsync(d => d.Id == romaneo.ListaMatanzaDetalleId, cancellationToken);
            if (renglon != null)
                renglon.CantidadFaenada = Math.Max(0, renglon.CantidadFaenada - 1);

            // Revertir Puntos de las tipificaciones usadas
            var codigos = romaneo.Piezas.Select(p => p.TipificacionId).ToList();
            var tipificaciones = await this.context.Tipificaciones
                .Where(t => codigos.Contains(t.Codigo))
                .ToListAsync(cancellationToken);
            foreach (var pieza in romaneo.Piezas)
            {
                var tip = tipificaciones.FirstOrDefault(t => t.Codigo == pieza.TipificacionId);
                if (tip != null)
                {
                    tip.Puntos = Math.Max(0, tip.Puntos - 1);
                    tip.FechaActualizacion = DateTime.Now;
                }
            }

            // El animal vuelve a En Pie: la tropa deja de estar FAENADA
            var tropa = await this.context.Tropas.FirstOrDefaultAsync(t => t.Id == romaneo.TropaId, cancellationToken);
            if (tropa != null && tropa.EstadoTropaId == EstadosTropa.Faenada)
                tropa.EstadoTropaId = EstadosTropa.Recepcionada;

            // Auditar la reversa (append-only)
            await TropaMovimientos.RegistrarAsync(
                this.context, romaneo.TropaId, TiposMovimientoTropa.Faena, EstadosTropa.Recepcionada,
                $"Anulacion de romaneo N° {romaneo.NumeroRomaneo}" +
                    (string.IsNullOrWhiteSpace(request.Motivo) ? "." : $": {request.Motivo}"),
                request.UsuarioId, "LISTA_MATANZA", lm.Id, cancellationToken);

            await this.context.SaveChangesAsync(cancellationToken);

            return new AnularRomaneoResponse { Id = romaneo.Id };
        }
    }
}
