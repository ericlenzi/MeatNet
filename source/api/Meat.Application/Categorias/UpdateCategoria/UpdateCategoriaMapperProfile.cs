using AutoMapper;
using Meat.Domain.Categorias;

namespace Meat.Application.Categorias.UpdateCategoria
{
    public class UpdateCategoriaMapperProfile : Profile
    {
        public UpdateCategoriaMapperProfile()
        {
            this.CreateMap<UpdateCategoriaRequest, Categoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Especie, opt => opt.Ignore())
                .ForMember(dest => dest.TipoSexo, opt => opt.Ignore())
                .ForMember(dest => dest.FechaActualizacion, opt => opt.Ignore());
        }
    }
}
