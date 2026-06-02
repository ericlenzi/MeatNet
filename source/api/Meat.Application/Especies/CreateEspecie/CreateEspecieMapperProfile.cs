using AutoMapper;
using Meat.Domain.Especies;

namespace Meat.Application.Especies.CreateEspecie
{
    public class CreateEspecieMapperProfile : Profile
    {
        public CreateEspecieMapperProfile()
        {
            this.CreateMap<CreateEspecieRequest, Especie>();
        }
    }
}
