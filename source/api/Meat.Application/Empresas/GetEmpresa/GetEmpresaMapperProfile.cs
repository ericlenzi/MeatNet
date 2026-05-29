using AutoMapper;
using Meat.Domain.Empresas;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaMapperProfile : Profile
    {
        public GetEmpresaMapperProfile()
        {
            this.CreateMap<Empresa, GetEmpresaResponse>();
        }
    }
}
