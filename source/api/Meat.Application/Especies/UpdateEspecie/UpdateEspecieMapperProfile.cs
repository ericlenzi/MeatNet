using AutoMapper;
using Meat.Domain.Especies;

namespace Meat.Application.Especies.UpdateEspecie
{
    public class UpdateEspecieMapperProfile : Profile
    {
        public UpdateEspecieMapperProfile()
        {
            this.CreateMap<UpdateEspecieRequest, Especie>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore());
        }
    }
}
