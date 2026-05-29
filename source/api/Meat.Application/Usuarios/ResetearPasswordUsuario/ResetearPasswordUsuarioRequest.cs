using MediatR;
using Meat.Application.Shared;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Usuarios.ResetearPasswordUsuario
{
    public class ResetearPasswordUsuarioRequest : RequestBase, IRequest<ResetearPasswordUsuarioResponse>
    {

        [Required]
        public string ContraseñaNueva { get; set; }
    }
}