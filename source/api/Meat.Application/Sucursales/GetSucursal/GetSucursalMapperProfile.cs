using Meat.Domain.Sucursales;

namespace Meat.Application.Sucursales.GetSucursal
{
    public class GetSucursalMapperProfile : AutoMapper.Profile
    {
        public GetSucursalMapperProfile()
        {
            this.CreateMap<Sucursal, GetSucursalResponse>();
        }
    }
}
