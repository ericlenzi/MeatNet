using AutoMapper;
using Meat.Domain.Parametros;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroMapperProfile : Profile
    {
        public CreateParametroMapperProfile()
        {
            this.CreateMap<CreateParametroRequest, Parametro>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore());
        }
    }
}
