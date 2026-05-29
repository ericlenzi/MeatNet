using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Usuarios.GetUsuarios
{
    public class GetUsuariosRequest : RequestListBase, IRequest<GetUsuariosResponse>
    {
        public int? Rol { get; set; }
        public int? Estado { get; set; }
    }
}
