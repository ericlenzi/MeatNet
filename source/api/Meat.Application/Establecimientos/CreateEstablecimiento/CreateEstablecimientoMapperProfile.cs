using AutoMapper;
using Meat.Domain.Establecimientos;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoMapperProfile : Profile
    {
        public CreateEstablecimientoMapperProfile()
        {
            this.CreateMap<CreateEstablecimientoRequest, Establecimiento>();
        }
    }
}
