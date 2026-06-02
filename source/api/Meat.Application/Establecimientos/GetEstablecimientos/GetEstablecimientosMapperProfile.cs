using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosMapperProfile : AutoMapper.Profile
    {
        public GetEstablecimientosMapperProfile()
        {
            this.CreateMap<Establecimiento, GetEstablecimientosItem>()
                .ForMember(d => d.SucursalNombre, c => c.MapFrom(s => s.Sucursal != null ? s.Sucursal.Nombre : ""))
                .ForMember(d => d.EspecieNombre, c => c.MapFrom(s => s.Especie != null ? s.Especie.Nombre : ""))
                .ForMember(d => d.EmpresaNombre, c => c.MapFrom(s => s.Empresa != null ? s.Empresa.Nombre : ""));
        }
    }
}