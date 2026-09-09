using MediatR;

namespace Meat.Application.TiposEspecies.GetTipoEspecie
{
    public class GetTipoEspecieRequest : IRequest<GetTipoEspecieResponse>
    {
        public string Codigo { get; set; }
    }
}
