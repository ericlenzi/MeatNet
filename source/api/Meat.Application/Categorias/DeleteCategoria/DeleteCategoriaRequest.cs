using MediatR;

namespace Meat.Application.Categorias.DeleteCategoria
{
    public class DeleteCategoriaRequest : IRequest<DeleteCategoriaResponse>
    {
        public string Id { get; set; }
    }
}
