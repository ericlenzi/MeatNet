using AutoMapper;
using Meat.Domain.Establecimientos;
using System.Linq;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoMapperProfile : Profile
    {
        public GetEstablecimientoMapperProfile()
        {
            this.CreateMap<Establecimiento, GetEstablecimientoResponse>()
                .ForMember(d => d.Especies, c => c.MapFrom(s =>
                    s.Especies != null
                        ? s.Especies.Select(ee => new EspecieItem { Id = ee.EspecieId, Nombre = ee.Especie != null ? ee.Especie.Nombre : "" })
                        : Enumerable.Empty<EspecieItem>()));
        }
    }
}
