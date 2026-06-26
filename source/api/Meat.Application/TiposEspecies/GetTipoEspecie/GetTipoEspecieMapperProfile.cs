using AutoMapper;
using Meat.Domain.TiposEspecies;

namespace Meat.Application.TiposEspecies.GetTipoEspecie
{
    public class GetTipoEspecieMapperProfile : Profile
    {
        public GetTipoEspecieMapperProfile()
        {
            this.CreateMap<TipoEspecie, GetTipoEspecieResponse>();
        }
    }
}
