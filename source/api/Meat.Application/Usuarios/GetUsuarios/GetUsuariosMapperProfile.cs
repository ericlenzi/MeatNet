using Meat.Domain.Usuarios;

namespace Meat.Application.Usuarios.GetUsuarios
{
    public class GetUsuariosMapperProfile : AutoMapper.Profile
    {
        public GetUsuariosMapperProfile()
        {
            this.CreateMap<Usuario, GetUsuariosItem>();
        }
    }
}
