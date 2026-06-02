using Meat.Domain.Clientes;

namespace Meat.Application.Clientes.GetCliente
{
    public class GetClienteMapperProfile : AutoMapper.Profile
    {
        public GetClienteMapperProfile()
        {
            this.CreateMap<Cliente, GetClienteResponse>();
        }
    }
}
