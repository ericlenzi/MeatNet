using Meat.Domain.Usuarios;
using Meat.Queries.Dtos;

namespace Meat.Application.Usuarios.GetUsuarios
{
    public class GetUsuariosMapperProfile : AutoMapper.Profile
    {
        public GetUsuariosMapperProfile()
        {
            this.CreateMap<Usuario, GetUsuariosItem>()
                .ForMember(d => d.UserName, c => c.MapFrom(s => s.UserName));
            this.CreateMap<UsuarioDto, GetUsuariosItem>();
        }
    }
}
