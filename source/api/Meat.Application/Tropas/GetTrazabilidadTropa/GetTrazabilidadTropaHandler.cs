using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tropas.GetTrazabilidadTropa
{
    public class GetTrazabilidadTropaHandler : IRequestHandler<GetTrazabilidadTropaRequest, GetTrazabilidadTropaResponse>
    {
        private readonly MeatContext context;

        public GetTrazabilidadTropaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTrazabilidadTropaResponse> Handle(GetTrazabilidadTropaRequest request, CancellationToken cancellationToken)
        {
            // El NumeroTropa es correlativo por Cliente-Establecimiento + Especie, por lo
            // que puede coincidir con mas de una tropa. Devolvemos todas las que matcheen
            // dentro de la empresa activa (y del establecimiento si se especifica).
            var tropas = await (
                from t in this.context.Tropas
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join esp in this.context.Especies on t.EspecieCodigo equals esp.Codigo
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join est in this.context.Establecimientos on i.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                join etr in this.context.TiposEstadosTropas on t.EstadoTropaId equals etr.Codigo
                where t.NumeroTropa == request.NumeroTropa
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || est.Id == request.EstablecimientoId)
                select new TrazabilidadTropaItem
                {
                    TropaId = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    EspecieCodigo = t.EspecieCodigo,
                    EspecieNombre = esp.Nombre,
                    ClienteNombre = c.Nombre,
                    EstablecimientoNombre = est.Nombre,
                    NumeroIngreso = i.NumeroIngreso,
                    EstadoTropaId = t.EstadoTropaId,
                    EstadoTropaNombre = etr.Nombre,
                    FechaRecepcion = t.FechaRecepcion
                })
                .OrderBy(x => x.EstablecimientoNombre)
                .ThenBy(x => x.EspecieNombre)
                .ThenBy(x => x.ClienteNombre)
                .ToListAsync(cancellationToken);

            if (tropas.Count == 0)
                return new GetTrazabilidadTropaResponse { Data = tropas };

            var tropaIds = tropas.Select(x => x.TropaId).ToList();

            // Fuente 1: log propio de la tropa (recepcion, ubicacion, anulacion, ...)
            var tropaMovs = await (
                from m in this.context.TropasMovimientos
                join u in this.context.Usuarios on m.UsuarioId equals (Guid?)u.Id into gu
                from u in gu.DefaultIfEmpty()
                where tropaIds.Contains(m.TropaId)
                select new
                {
                    m.TropaId,
                    m.Fecha,
                    m.TipoMovimiento,
                    m.EstadoResultanteId,
                    m.Detalle,
                    UsuarioNombre = u != null ? (u.Nombre + " " + u.Apellido) : null
                })
                .ToListAsync(cancellationToken);

            // Fuente 2: historial de la planificacion (ListaMatanzaMovimiento) por tropa
            var planMovs = await (
                from mm in this.context.ListasMatanzasMovimientos
                join lm in this.context.ListasMatanzas on mm.ListaMatanzaId equals lm.Id
                join u in this.context.Usuarios on mm.UsuarioId equals u.Id into gu
                from u in gu.DefaultIfEmpty()
                where mm.TropaId != null && tropaIds.Contains(mm.TropaId.Value)
                select new
                {
                    TropaId = mm.TropaId.Value,
                    mm.Fecha,
                    mm.TipoMovimiento,
                    mm.Motivo,
                    lm.NumeroLista,
                    UsuarioNombre = u != null ? (u.Nombre + " " + u.Apellido) : null
                })
                .ToListAsync(cancellationToken);

            // Unificar ambas fuentes en una sola linea de tiempo por tropa, ordenada por fecha.
            var eventos = new List<KeyValuePair<Guid, TrazabilidadMovimientoItem>>();

            foreach (var m in tropaMovs)
            {
                eventos.Add(new KeyValuePair<Guid, TrazabilidadMovimientoItem>(m.TropaId, new TrazabilidadMovimientoItem
                {
                    Fecha = m.Fecha,
                    Fase = m.TipoMovimiento == TiposMovimientoTropa.Anulacion ? "Anulación" : "Recepción",
                    TipoMovimiento = m.TipoMovimiento,
                    EstadoResultanteId = m.EstadoResultanteId,
                    Detalle = m.Detalle,
                    Referencia = null,
                    UsuarioNombre = m.UsuarioNombre
                }));
            }

            foreach (var m in planMovs)
            {
                eventos.Add(new KeyValuePair<Guid, TrazabilidadMovimientoItem>(m.TropaId, new TrazabilidadMovimientoItem
                {
                    Fecha = m.Fecha,
                    Fase = "Planificación",
                    TipoMovimiento = m.TipoMovimiento,
                    EstadoResultanteId = null,
                    Detalle = m.Motivo,
                    Referencia = $"LM #{m.NumeroLista}",
                    UsuarioNombre = m.UsuarioNombre
                }));
            }

            var eventosPorTropa = eventos
                .GroupBy(x => x.Key)
                .ToDictionary(
                    g => g.Key,
                    g => (IEnumerable<TrazabilidadMovimientoItem>)g.Select(x => x.Value).OrderBy(i => i.Fecha).ToList());

            foreach (var tropa in tropas)
            {
                if (eventosPorTropa.TryGetValue(tropa.TropaId, out var lista))
                    tropa.Movimientos = lista;
            }

            return new GetTrazabilidadTropaResponse { Data = tropas };
        }
    }
}
