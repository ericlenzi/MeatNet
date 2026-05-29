using AutoMapper;
using Meat.Domain.Puestos;

namespace Meat.Application.Puestos.UpdatePuesto
{

    public class UpdatePuestoMapperProfile : Profile
    {
        public UpdatePuestoMapperProfile()
        {
            this.CreateMap<UpdatePuestoRequest, Puesto>();
        }
    }
}
