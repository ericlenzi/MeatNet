using MediatR;

namespace Meat.Application.TiposEspecies.DeleteTipoEspecie
{
    public class DeleteTipoEspecieRequest : IRequest<DeleteTipoEspecieResponse>
    {
        public string Id { get; set; }
    }
}
