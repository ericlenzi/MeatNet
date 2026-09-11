using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.TiposPuestos.UpdateTipoPuesto
{
    public class UpdateTipoPuestoRequest : IRequest<UpdateTipoPuestoResponse>
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
