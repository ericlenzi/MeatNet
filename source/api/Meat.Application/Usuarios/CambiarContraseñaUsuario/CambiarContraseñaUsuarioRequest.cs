using MediatR;
using Meat.Application.Shared;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Usuarios.CambiarContraseñaUsuario
{
    public class CambiarContraseñaUsuarioRequest : RequestBase, IRequest<CambiarContraseñaUsuarioResponse>
    {
        [Required]
        public string ContraseñaActual { get; set; }

        [Required]
        public string ContraseñaNueva { get; set; }
    }
}