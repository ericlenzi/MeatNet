using Meat.Domain.Usuarios;

namespace Meat.Application.Usuarios.GetUsuario
{
    public class GetUsuarioMapperProfile : AutoMapper.Profile
    {
        public GetUsuarioMapperProfile()
        {
            this.CreateMap<Usuario, GetUsuarioResponse>()
                .ForMember(d => d.UserName, c => c.MapFrom(s => s.UserName));
        }
    }
}
