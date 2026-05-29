using AutoMapper;
using Meat.Domain.ParametrosSucursales;

namespace Meat.Application.ParametrosSucursales.UpdateParametroSucursal
{
    public class UpdateParametroSucursalMapperProfile : Profile
    {
        public UpdateParametroSucursalMapperProfile()
        {
            this.CreateMap<UpdateParametroSucursalRequest, ParametroSucursal>();
        }
    }
}
