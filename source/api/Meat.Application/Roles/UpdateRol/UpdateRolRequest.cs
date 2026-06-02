using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Roles.UpdateRol
{
    public class UpdateRolRequest : IRequest<UpdateRolResponse>
    {
        public string Codigo { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string CodigoEmpresa { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}
