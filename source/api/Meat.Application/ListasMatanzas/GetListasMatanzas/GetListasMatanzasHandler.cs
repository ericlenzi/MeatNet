using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.GetListasMatanzas
{
    public class GetListasMatanzasHandler : IRequestHandler<GetListasMatanzasRequest, GetListasMatanzasResponse>
    {
        private readonly MeatContext context;

        public GetListasMatanzasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetListasMatanzasResponse> Handle(GetListasMatanzasRequest request, CancellationToken cancellationToken)
        {
            IQueryable<ListaMatanza> queryable = this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Especie)
                .Include(lm => lm.EstadoListaMatanza)
                .Include(lm => lm.Renglones)
                .Where(lm => lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (request.EstablecimientoId.HasValue)
                queryable = queryable.Where(lm => lm.EstablecimientoId == request.EstablecimientoId.Value);

            if (!string.IsNullOrEmpty(request.EspecieId))
                queryable = queryable.Where(lm => lm.EspecieId == request.EspecieId);

            if (!string.IsNullOrEmpty(request.EstadoListaMatanzaId))
                queryable = queryable.Where(lm => lm.EstadoListaMatanzaId == request.EstadoListaMatanzaId);

            if (request.Fecha.HasValue)
            {
                var fecha = request.Fecha.Value.Date;
                queryable = queryable.Where(lm => lm.Fecha == fecha);
            }

            queryable = queryable.OrderByDescending(lm => lm.Fecha).ThenByDescending(lm => lm.NumeroLista);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable
                .Page(request.PageSize, request.PageIndex)
                .Select(lm => new ListaMatanzaListItem
                {
                    Id = lm.Id,
                    NumeroLista = lm.NumeroLista,
                    Fecha = lm.Fecha,
                    EstablecimientoId = lm.EstablecimientoId,
                    EstablecimientoNombre = lm.Establecimiento.Nombre,
                    EspecieId = lm.EspecieId,
                    EspecieNombre = lm.Especie.Nombre,
                    EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                    EstadoListaMatanzaNombre = lm.EstadoListaMatanza.Nombre,
                    Version = lm.Version,
                    TotalRenglones = lm.Renglones.Count,
                    TotalCabezas = lm.Renglones.Sum(r => (int?)r.Cantidad) ?? 0
                })
                .ToListAsync(cancellationToken);

            return new GetListasMatanzasResponse
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
