using Meat.Domain.Sucursales;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoMapperProfile : AutoMapper.Profile
    {
        public GetSucursalByCodigoMapperProfile()
        {
            this.CreateMap<Sucursal, GetSucursalByCodigoResponse>()
                .ForMember(dest => dest.CodigoEmpresa, opt => opt.MapFrom(src => src.Empresa.CodigoEmpresa));
        }
    }
}
