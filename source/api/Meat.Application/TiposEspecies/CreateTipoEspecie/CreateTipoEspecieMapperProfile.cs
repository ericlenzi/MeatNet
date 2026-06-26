using AutoMapper;
using Meat.Domain.TiposEspecies;

namespace Meat.Application.TiposEspecies.CreateTipoEspecie
{
    public class CreateTipoEspecieMapperProfile : Profile
    {
        public CreateTipoEspecieMapperProfile()
        {
            this.CreateMap<CreateTipoEspecieRequest, TipoEspecie>()
                .ForMember(dest => dest.Especie, opt => opt.Ignore())
                .ForMember(dest => dest.TipoSexo, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());
        }
    }
}
