using AutoMapper;
using Meat.Domain.TiposEspecies;

namespace Meat.Application.TiposEspecies.GetTiposEspecies
{
    public class GetTiposEspeciesMapperProfile : Profile
    {
        public GetTiposEspeciesMapperProfile()
        {
            this.CreateMap<TipoEspecie, GetTiposEspeciesItem>()
                .ForMember(d => d.EspecieNombre, c => c.MapFrom(s => s.Especie != null ? s.Especie.Nombre : string.Empty))
                .ForMember(d => d.TipoSexoNombre, c => c.MapFrom(s => s.TipoSexo != null ? s.TipoSexo.Nombre : string.Empty));
        }
    }
}
