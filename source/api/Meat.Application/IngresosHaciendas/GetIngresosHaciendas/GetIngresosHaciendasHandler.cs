using MediatR;
using Meat.Application.Shared;
using Meat.Domain.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.GetIngresosHaciendas
{
    public class GetIngresosHaciendasHandler : IRequestHandler<GetIngresosHaciendasRequest, GetIngresosHaciendasResponse>
    {
        private readonly MeatContext context;

        public GetIngresosHaciendasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetIngresosHaciendasResponse> Handle(GetIngresosHaciendasRequest request, CancellationToken cancellationToken)
        {
            IQueryable<IngresoHacienda> queryable = this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Cliente)
                .Include(i => i.EstadoIngreso)
                .Include(i => i.Ubicaciones)
                .Where(i => i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (request.EstablecimientoId.HasValue)
                queryable = queryable.Where(i => i.EstablecimientoId == request.EstablecimientoId.Value);

            if (!string.IsNullOrEmpty(request.EstadoIngresoId))
                queryable = queryable.Where(i => i.EstadoIngresoId == request.EstadoIngresoId);

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(i =>
                    i.NumeroDte.Contains(request.Filter) ||
                    i.Cliente.Nombre.Contains(request.Filter));

            queryable = queryable.OrderByDescending(i => i.FechaHoraIngreso);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable
                .Page(request.PageSize, request.PageIndex)
                .Select(i => new IngresoHaciendaListItem
                {
                    Id = i.Id,
                    NumeroIngreso = i.NumeroIngreso,
                    FechaHoraIngreso = i.FechaHoraIngreso,
                    NumeroDte = i.NumeroDte,
                    EstablecimientoId = i.EstablecimientoId,
                    EstablecimientoNombre = i.Establecimiento.Nombre,
                    ClienteNombre = i.Cliente.Nombre,
                    EstadoIngresoId = i.EstadoIngresoId,
                    EstadoIngresoNombre = i.EstadoIngreso.Nombre,
                    TotalCabezas = i.Ubicaciones.Sum(u => (int?)u.Cantidad) ?? 0,
                    PesoNeto = i.PesoNeto
                })
                .ToListAsync(cancellationToken);

            return new GetIngresosHaciendasResponse
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
