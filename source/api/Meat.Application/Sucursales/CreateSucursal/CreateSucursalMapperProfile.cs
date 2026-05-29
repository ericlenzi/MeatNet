using AutoMapper;
using Meat.Domain.Sucursales;

namespace Meat.Application.Sucursales.CreateSucursal
{

    public class CreateSucursalMapperProfile : Profile
    {
        public CreateSucursalMapperProfile()
        {
            this.CreateMap<CreateSucursalRequest, Sucursal>();
        }
    }
}
