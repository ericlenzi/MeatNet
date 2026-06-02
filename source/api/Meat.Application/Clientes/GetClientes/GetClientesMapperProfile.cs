using Meat.Domain.Clientes;

namespace Meat.Application.Clientes.GetClientes
{
    public class GetClientesMapperProfile : AutoMapper.Profile
    {
        public GetClientesMapperProfile()
        {
            this.CreateMap<Cliente, GetClientesItem>()
                .ForMember(d => d.TipoClienteNombre, c => c.MapFrom(s => s.TipoCliente != null ? s.TipoCliente.Nombre : ""));
        }
    }
}
