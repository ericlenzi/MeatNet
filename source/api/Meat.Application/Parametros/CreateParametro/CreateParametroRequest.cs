using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroRequest : IRequest<CreateParametroResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Valor { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
