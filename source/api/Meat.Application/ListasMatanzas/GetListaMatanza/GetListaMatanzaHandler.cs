using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.GetListaMatanza
{
    public class GetListaMatanzaHandler : IRequestHandler<GetListaMatanzaRequest, GetListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public GetListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetListaMatanzaResponse> Handle(GetListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Especie)
                .Include(lm => lm.EstadoListaMatanza)
                .Include(lm => lm.Renglones).ThenInclude(r => r.Tropa)
                .Include(lm => lm.Renglones).ThenInclude(r => r.Almacen)
                .Include(lm => lm.Movimientos)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                return null;

            return new GetListaMatanzaResponse
            {
                Id = entity.Id,
                NumeroLista = entity.NumeroLista,
                Fecha = entity.Fecha,
                EstablecimientoId = entity.EstablecimientoId,
                EstablecimientoNombre = entity.Establecimiento?.Nombre,
                EspecieId = entity.EspecieId,
                EspecieNombre = entity.Especie?.Nombre,
                EstadoListaMatanzaId = entity.EstadoListaMatanzaId,
                EstadoListaMatanzaNombre = entity.EstadoListaMatanza?.Nombre,
                Version = entity.Version,
                FechaConfirmacion = entity.FechaConfirmacion,
                FechaInicioEjecucion = entity.FechaInicioEjecucion,
                FechaFinalizacion = entity.FechaFinalizacion,
                Renglones = entity.Renglones
                    .OrderBy(r => r.Secuencia)
                    .Select(r => new RenglonItem
                    {
                        Id = r.Id,
                        TropaId = r.TropaId,
                        NumeroTropa = r.Tropa != null ? r.Tropa.NumeroTropa : 0,
                        AlmacenId = r.AlmacenId,
                        AlmacenNombre = r.Almacen?.Nombre,
                        Secuencia = r.Secuencia,
                        Cantidad = r.Cantidad,
                        CantidadFaenada = r.CantidadFaenada
                    }).ToList(),
                Movimientos = entity.Movimientos
                    .OrderBy(m => m.Fecha)
                    .Select(m => new MovimientoItem
                    {
                        Id = m.Id,
                        Version = m.Version,
                        Fecha = m.Fecha,
                        UsuarioId = m.UsuarioId,
                        TipoMovimiento = m.TipoMovimiento,
                        TropaId = m.TropaId,
                        AlmacenId = m.AlmacenId,
                        CantidadAnterior = m.CantidadAnterior,
                        CantidadNueva = m.CantidadNueva,
                        SecuenciaAnterior = m.SecuenciaAnterior,
                        SecuenciaNueva = m.SecuenciaNueva,
                        Motivo = m.Motivo
                    }).ToList()
            };
        }
    }
}
