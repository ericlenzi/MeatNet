using AutoMapper;
using Meat.Domain.Empresas;

namespace Meat.Application.Empresas.CreateEmpresa
{
    public class CreateEmpresaMapperProfile : Profile
    {
        public CreateEmpresaMapperProfile()
        {
            this.CreateMap<CreateEmpresaRequest, Empresa>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaPadre, opt => opt.Ignore());
        }
    }
}
