using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.Tipificaciones.GetTipificacion
{
    public class GetTipificacionRequest : IRequest<GetTipificacionResponse>
    {
        public string Codigo { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
