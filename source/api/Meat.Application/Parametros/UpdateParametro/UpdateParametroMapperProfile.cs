using AutoMapper;
using Meat.Domain.Parametros;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroMapperProfile : Profile
    {
        public UpdateParametroMapperProfile()
        {
            this.CreateMap<UpdateParametroRequest, Parametro>()
                .ForMember(dest => dest.Codigo, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore());
        }
    }
}
