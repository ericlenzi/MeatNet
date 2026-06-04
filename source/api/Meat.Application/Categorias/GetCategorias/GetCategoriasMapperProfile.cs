using AutoMapper;
using Meat.Domain.Categorias;

namespace Meat.Application.Categorias.GetCategorias
{
    public class GetCategoriasMapperProfile : Profile
    {
        public GetCategoriasMapperProfile()
        {
            this.CreateMap<Categoria, GetCategoriasItem>()
                .ForMember(d => d.EspecieNombre, c => c.MapFrom(s => s.Especie != null ? s.Especie.Nombre : string.Empty))
                .ForMember(d => d.TipoSexoNombre, c => c.MapFrom(s => s.TipoSexo != null ? s.TipoSexo.Nombre : string.Empty));
        }
    }
}
