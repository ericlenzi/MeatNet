using AutoMapper;
using Meat.Domain.Categorias;

namespace Meat.Application.Categorias.CreateCategoria
{
    public class CreateCategoriaMapperProfile : Profile
    {
        public CreateCategoriaMapperProfile()
        {
            this.CreateMap<CreateCategoriaRequest, Categoria>()
                .ForMember(dest => dest.Especie, opt => opt.Ignore())
                .ForMember(dest => dest.TipoSexo, opt => opt.Ignore())
                .ForMember(dest => dest.Activo, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());
        }
    }
}
