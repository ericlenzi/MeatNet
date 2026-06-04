using AutoMapper;
using Meat.Domain.Clientes;

namespace Meat.Application.Clientes.CreateCliente
{
    public class CreateClienteMapperProfile : Profile
    {
        public CreateClienteMapperProfile()
        {
            this.CreateMap<CreateClienteRequest, Cliente>()
                .ForMember(dest => dest.TipoCliente, opt => opt.Ignore());
        }
    }
}
