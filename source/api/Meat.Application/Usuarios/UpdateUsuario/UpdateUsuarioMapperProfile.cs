using AutoMapper;
using Meat.Domain.Usuarios;

namespace Meat.Application.Usuarios.UpdateUsuario
{

    public class UpdateUsuarioMapperProfile : Profile
    {
        public UpdateUsuarioMapperProfile()
        {
            this.CreateMap<UpdateUsuarioRequest, Usuario>();
        }
    }
}
