using AutoMapper;
using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoMapperProfile : Profile
    {
        public UpdateEstablecimientoMapperProfile()
        {
            this.CreateMap<UpdateEstablecimientoRequest, Establecimiento>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore());
        }
    }
}
