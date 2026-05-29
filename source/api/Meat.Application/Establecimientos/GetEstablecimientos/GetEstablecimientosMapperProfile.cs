using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosMapperProfile : AutoMapper.Profile
    {
        public GetEstablecimientosMapperProfile()
        {
            this.CreateMap<Establecimiento, GetEstablecimientosItem>();
        //        .ForMember(d => d.DireccionCompleta, c => c.MapFrom(s => $"{s.Direccion.Calle} {s.Direccion.Numero}"));
        }
    }
}