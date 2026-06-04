using AutoMapper;
using Meat.Domain.Categorias;

namespace Meat.Application.Categorias.GetCategoria
{
    public class GetCategoriaMapperProfile : Profile
    {
        public GetCategoriaMapperProfile()
        {
            this.CreateMap<Categoria, GetCategoriaResponse>();
        }
    }
}
