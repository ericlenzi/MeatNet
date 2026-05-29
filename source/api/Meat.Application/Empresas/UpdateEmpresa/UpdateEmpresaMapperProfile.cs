using AutoMapper;
using Meat.Domain.Empresas;

namespace Meat.Application.Empresas.UpdateEmpresa
{
    public class UpdateEmpresaMapperProfile : Profile
    {
        public UpdateEmpresaMapperProfile()
        {
            this.CreateMap<UpdateEmpresaRequest, Empresa>();
        }
    }
}
