using AutoMapper;
using Meat.Domain.Empresas;

namespace Meat.Application.Empresas.CreateEmpresa
{
    public class CreateEmpresaMapperProfile : Profile
    {
        public CreateEmpresaMapperProfile()
        {
            this.CreateMap<CreateEmpresaRequest, Empresa>();
        }
    }
}
