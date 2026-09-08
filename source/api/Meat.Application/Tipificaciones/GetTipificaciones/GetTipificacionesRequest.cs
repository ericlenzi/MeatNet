using System;
using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Tipificaciones.GetTipificaciones
{
    // Hereda EmpresaId de RequestBase; el controller lo setea desde CurrentUser.
    public class GetTipificacionesRequest : RequestListBase, IRequest<GetTipificacionesResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
        public Guid? TipoEspecieId { get; set; }
    }
}
