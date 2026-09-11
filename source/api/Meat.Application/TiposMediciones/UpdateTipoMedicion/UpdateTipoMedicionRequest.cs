using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.TiposMediciones.UpdateTipoMedicion
{
    public class UpdateTipoMedicionRequest : IRequest<UpdateTipoMedicionResponse>
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
