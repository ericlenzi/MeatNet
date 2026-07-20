using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Tipificaciones.GetTipificaciones
{
    // Hereda CodigoEmpresa de RequestBase; el controller lo setea desde CurrentUser.
    public class GetTipificacionesRequest : RequestListBase, IRequest<GetTipificacionesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
        public string TipoEspecieId { get; set; }
    }
}
