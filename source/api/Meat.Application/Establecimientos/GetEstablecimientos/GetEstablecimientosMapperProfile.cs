using Meat.Domain.Establecimientos;
using System.Linq;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosMapperProfile : AutoMapper.Profile
    {
        public GetEstablecimientosMapperProfile()
        {
            this.CreateMap<Establecimiento, GetEstablecimientosItem>()
                .ForMember(d => d.SucursalNombre, c => c.MapFrom(s => s.Sucursal != null ? s.Sucursal.Nombre : ""))
                .ForMember(d => d.EmpresaNombre, c => c.MapFrom(s => s.Empresa != null ? s.Empresa.Nombre : ""))
                .ForMember(d => d.Especies, c => c.MapFrom(s =>
                    s.Especies != null
                        ? s.Especies.Select(ee => new EspecieItem { Id = ee.EspecieId, Nombre = ee.Especie != null ? ee.Especie.Nombre : "" })
                        : Enumerable.Empty<EspecieItem>()));
        }
    }
}