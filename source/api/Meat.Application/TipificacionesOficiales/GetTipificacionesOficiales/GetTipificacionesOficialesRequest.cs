using MediatR;

namespace Meat.Application.TipificacionesOficiales.GetTipificacionesOficiales
{
    public class GetTipificacionesOficialesRequest : IRequest<GetTipificacionesOficialesResponse>
    {
        /// <summary>Filtro opcional por especie.</summary>
        public string EspecieId { get; set; }
    }
}
