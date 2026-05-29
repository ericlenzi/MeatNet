using Meat.Domain.Puestos;

namespace Meat.Application.Puestos.GetPuesto
{
    public class GetPuestoMapperProfile : AutoMapper.Profile
    {
        public GetPuestoMapperProfile()
        {
            this.CreateMap<Puesto, GetPuestoResponse>();
        }
    }
}