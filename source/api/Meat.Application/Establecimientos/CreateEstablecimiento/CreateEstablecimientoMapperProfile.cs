using AutoMapper;
using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoMapperProfile : Profile
    {
        public CreateEstablecimientoMapperProfile()
        {
            this.CreateMap<CreateEstablecimientoRequest, Establecimiento>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore())
                .ForMember(dest => dest.Especies, opt => opt.Ignore());
        }
    }
}
