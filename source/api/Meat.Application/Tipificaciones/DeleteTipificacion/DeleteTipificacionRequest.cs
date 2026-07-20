using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.Tipificaciones.DeleteTipificacion
{
    public class DeleteTipificacionRequest : IRequest<DeleteTipificacionResponse>
    {
        public string Codigo { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
