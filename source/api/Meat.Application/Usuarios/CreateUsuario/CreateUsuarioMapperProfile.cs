using AutoMapper;
using Meat.Domain.Usuarios;

namespace Meat.Application.Usuarios.CreateUsuario
{

    public class CreateUsuarioMapperProfile : Profile
    {
        public CreateUsuarioMapperProfile()
        {
            this.CreateMap<CreateUsuarioRequest, Usuario>();
        }
    }
}
