using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.GetTipificadores
{
    public class GetTipificadoresHandler : IRequestHandler<GetTipificadoresRequest, GetTipificadoresResponse>
    {
        private readonly MeatContext context;

        public GetTipificadoresHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipificadoresResponse> Handle(GetTipificadoresRequest request, CancellationToken cancellationToken)
        {
            // Sin filtro por empresa: lo aplica el query filter global del MeatContext.
            var queryable =
                from t in this.context.Tipificadores
                join est in this.context.Establecimientos on t.EstablecimientoId equals est.Id
                join e in this.context.Especies on t.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where (request.EstablecimientoId == null || t.EstablecimientoId == request.EstablecimientoId)
                    && (request.EspecieId == null || t.EspecieId == request.EspecieId)
                    && (request.Estado == null || t.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || t.Nombre.Contains(request.Filter)
                        || t.Matricula.Contains(request.Filter))
                // El de por defecto primero: es el que el palco usa todos los dias.
                orderby t.PorDefecto descending, t.Nombre
                select new TipificadorItem
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Matricula = t.Matricula,
                    EstablecimientoId = t.EstablecimientoId,
                    EstablecimientoNombre = est.Nombre,
                    EspecieId = t.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    PorDefecto = t.PorDefecto,
                    Activo = t.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTipificadoresResponse { Data = data, TotalRows = totalRows };
        }
    }
}
