using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Categorias.GetCategorias
{
    public class GetCategoriasRequest : RequestListBase, IRequest<GetCategoriasResponse>
    {
        public bool? Estado { get; set; }
        public string EspecieId { get; set; }
    }
}
