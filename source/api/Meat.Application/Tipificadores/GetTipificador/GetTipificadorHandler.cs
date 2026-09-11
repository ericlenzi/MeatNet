using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.GetTipificador
{
    public class GetTipificadorHandler : IRequestHandler<GetTipificadorRequest, GetTipificadorResponse>
    {
        private readonly MeatContext context;

        public GetTipificadorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipificadorResponse> Handle(GetTipificadorRequest request, CancellationToken cancellationToken)
        {
            return await (
                from t in this.context.Tipificadores
                join est in this.context.Establecimientos on t.EstablecimientoId equals est.Id
                join e in this.context.Especies on t.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where t.Id == request.Id
                select new GetTipificadorResponse
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
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
