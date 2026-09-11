using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.GetPuesto
{
    public class GetPuestoHandler : IRequestHandler<GetPuestoRequest, GetPuestoResponse>
    {
        private readonly MeatContext context;

        public GetPuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetPuestoResponse> Handle(GetPuestoRequest request, CancellationToken cancellationToken)
        {
            return await (
                from p in this.context.Puestos
                join est in this.context.Establecimientos on p.EstablecimientoId equals est.Id
                join e in this.context.Especies on p.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where p.Id == request.Id
                select new GetPuestoResponse
                {
                    Id = p.Id,
                    CodigoPuesto = p.CodigoPuesto,
                    Nombre = p.Nombre,
                    EstablecimientoId = p.EstablecimientoId,
                    EstablecimientoNombre = est.Nombre,
                    EspecieId = p.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    TipoPuestoId = p.TipoPuestoId,
                    TipoMedicionId = p.TipoMedicionId,
                    Activo = p.Activo
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
