using AutoMapper;
using Meat.Domain.Roles;

namespace Meat.Application.Roles.GetRol
{
    public class GetRolMapperProfile : Profile
    {
        public GetRolMapperProfile()
        {
            this.CreateMap<Rol, GetRolResponse>();
        }
    }
}
