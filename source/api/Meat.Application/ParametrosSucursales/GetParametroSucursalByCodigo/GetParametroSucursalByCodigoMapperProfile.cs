using Meat.Domain.ParametrosSucursales;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursalByCodigo
{
    public class GetParametroSucursalByCodigoMapperProfile : AutoMapper.Profile
    {
        public GetParametroSucursalByCodigoMapperProfile()
        {
            this.CreateMap<ParametroSucursal, GetParametroSucursalByCodigoResponse>();
        }
    }
}
