using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TipificacionesOficiales.GetTipificacionesOficiales
{
    public class GetTipificacionesOficialesHandler : IRequestHandler<GetTipificacionesOficialesRequest, GetTipificacionesOficialesResponse>
    {
        private readonly MeatContext context;

        public GetTipificacionesOficialesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipificacionesOficialesResponse> Handle(GetTipificacionesOficialesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TipificacionesOficiales
                .Where(t => t.Activo)
                .Where(t => string.IsNullOrEmpty(request.EspecieId) || t.EspecieId == request.EspecieId)
                .OrderBy(t => t.Nombre)
                .Select(t => new TipificacionOficialItem { Codigo = t.Codigo, Nombre = t.Nombre, EspecieId = t.EspecieId })
                .ToListAsync(cancellationToken);

            return new GetTipificacionesOficialesResponse { Data = data };
        }
    }
}
