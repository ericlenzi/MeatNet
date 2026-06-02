using AutoMapper;
using Meat.Domain.Clientes;

namespace Meat.Application.Clientes.UpdateCliente
{
    public class UpdateClienteMapperProfile : Profile
    {
        public UpdateClienteMapperProfile()
        {
            this.CreateMap<UpdateClienteRequest, Cliente>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CodigoCliente, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaPadre, opt => opt.Ignore())
                .ForMember(dest => dest.TipoCliente, opt => opt.Ignore());
        }
    }
}
