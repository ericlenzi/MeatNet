using MediatR;

namespace Meat.Application.Categorias.GetCategoria
{
    public class GetCategoriaRequest : IRequest<GetCategoriaResponse>
    {
        public string Id { get; set; }
    }
}
