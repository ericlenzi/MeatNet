using AutoMapper;
using Meat.Domain.TiposEspecies;

namespace Meat.Application.TiposEspecies.UpdateTipoEspecie
{
    public class UpdateTipoEspecieMapperProfile : Profile
    {
        public UpdateTipoEspecieMapperProfile()
        {
            this.CreateMap<UpdateTipoEspecieRequest, TipoEspecie>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.Especie, opt => opt.Ignore())
                .ForMember(dest => dest.TipoSexo, opt => opt.Ignore());
        }
    }
}
