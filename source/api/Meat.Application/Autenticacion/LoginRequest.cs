using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Autenticacion
{
    public class LoginRequest : IRequest<LoginResponse>
    {
        [Required]
        public string Usuario { get; set; }
        [Required]
        public string Contraseña { get; set; }

        //POS
        //[Required]
        //public string NumeroSucursal { get; set; }
        //[Required]
        //public string NumeroPuesto { get; set; }
        //public string VersionPos { get; set; }

        //MEAT
        //public bool EsModoAdmin { get; set; }
    }
}
