using MediatR;

namespace Meat.Application.TiposEspecies.GetTipoEspecie
{
    public class GetTipoEspecieRequest : IRequest<GetTipoEspecieResponse>
    {
        public string Id { get; set; }
    }
}
