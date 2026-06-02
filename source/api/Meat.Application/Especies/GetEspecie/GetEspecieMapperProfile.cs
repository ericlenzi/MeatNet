using Meat.Domain.Especies;

namespace Meat.Application.Especies.GetEspecie
{
    public class GetEspecieMapperProfile : AutoMapper.Profile
    {
        public GetEspecieMapperProfile()
        {
            this.CreateMap<Especie, GetEspecieResponse>();
        }
    }
}
