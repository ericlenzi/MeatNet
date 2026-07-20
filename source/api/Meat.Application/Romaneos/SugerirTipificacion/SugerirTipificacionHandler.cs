using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.SugerirTipificacion
{
    /// <summary>
    /// Propone la Tipificacion para una pieza: filtro duro por Especie + TipoEspecie +
    /// UnidadFaena + DestinoComercial(si viene) + Activo, ordenado por Puntos desc. Si viene el
    /// peso, la propuesta es la de mayor Puntos cuyo rango [PesoDesde, PesoHasta] lo contiene.
    /// Devuelve tambien las candidatas para el combo (override manual).
    /// </summary>
    public class SugerirTipificacionHandler : IRequestHandler<SugerirTipificacionRequest, SugerirTipificacionResponse>
    {
        private readonly MeatContext context;

        public SugerirTipificacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<SugerirTipificacionResponse> Handle(SugerirTipificacionRequest request, CancellationToken cancellationToken)
        {
            var candidatas = await (
                from tip in this.context.Tipificaciones
                join dc in this.context.DestinosComerciales on tip.DestinoComercialId equals dc.Codigo into dcj
                from dc in dcj.DefaultIfEmpty()
                where tip.Activo
                    && tip.CodigoEmpresa == request.CodigoEmpresa
                    && tip.EspecieId == request.EspecieId
                    && tip.TipoEspecieId == request.TipoEspecieId
                    && tip.UnidadFaenaId == request.UnidadFaenaId
                    && (string.IsNullOrEmpty(request.DestinoComercialId) || tip.DestinoComercialId == request.DestinoComercialId)
                orderby tip.Puntos descending, tip.Descripcion
                select new TipificacionCandidata
                {
                    Codigo = tip.Codigo,
                    Descripcion = tip.Descripcion,
                    DestinoComercialId = tip.DestinoComercialId,
                    DestinoComercialNombre = dc != null ? dc.Nombre : null,
                    PesoDesde = tip.PesoDesde,
                    PesoHasta = tip.PesoHasta,
                    Puntos = tip.Puntos
                })
                .ToListAsync(cancellationToken);

            string propuesta = null;
            if (request.Peso.HasValue)
            {
                var p = request.Peso.Value;
                propuesta = candidatas
                    .Where(c => c.PesoDesde <= p && p <= c.PesoHasta)
                    .Select(c => c.Codigo)
                    .FirstOrDefault();
            }

            return new SugerirTipificacionResponse
            {
                PropuestaCodigo = propuesta,
                Candidatas = candidatas
            };
        }
    }
}
