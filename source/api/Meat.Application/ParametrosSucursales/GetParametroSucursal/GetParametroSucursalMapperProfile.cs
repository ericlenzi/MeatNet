using Meat.Domain.ParametrosSucursales;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursal
{
    public class GetParametroSucursalMapperProfile : AutoMapper.Profile
    {
        public GetParametroSucursalMapperProfile()
        {
            this.CreateMap<ParametroSucursal, GetParametroSucursalResponse>();
        }
    }
}
