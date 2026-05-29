using AutoMapper;
using Meat.Domain.Puestos;

namespace Meat.Application.Puestos.CreatePuesto
{

    public class CreatePuestoMapperProfile : Profile
    {
        public CreatePuestoMapperProfile()
        {
            this.CreateMap<CreatePuestoRequest, Puesto>();
        }
    }
}
