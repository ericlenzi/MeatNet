using AutoMapper;
using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoMapperProfile : Profile
    {
        public GetEstablecimientoMapperProfile()
        {
            this.CreateMap<Establecimiento, GetEstablecimientoResponse>();
        }
    }
}
