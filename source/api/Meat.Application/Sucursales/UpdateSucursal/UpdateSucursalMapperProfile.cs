using AutoMapper;
using Meat.Domain.Sucursales;

namespace Meat.Application.Sucursales.UpdateSucursal
{

    public class UpdateSucursalMapperProfile : Profile
    {
        public UpdateSucursalMapperProfile()
        {
            this.CreateMap<UpdateSucursalRequest, Sucursal>();
        }
    }
}
