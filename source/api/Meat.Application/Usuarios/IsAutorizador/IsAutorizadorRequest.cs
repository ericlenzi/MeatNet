using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Usuarios.IsAutorizador
{
    public class IsAutorizadorRequest : IRequest<IsAutorizadorResponse>
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Contraseña { get; set; }
        [Required]
        public string NumeroSucursal { get; set; }
    }
}
