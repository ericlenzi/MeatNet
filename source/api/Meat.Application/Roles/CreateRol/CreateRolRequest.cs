using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Roles.CreateRol
{
    public class CreateRolRequest : IRequest<CreateRolResponse>
    {
        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Activo { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
