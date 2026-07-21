using MediatR;

namespace Meat.Application.UnidadesFaenas.GetUnidadFaena
{
    public class GetUnidadFaenaRequest : IRequest<GetUnidadFaenaResponse>
    {
        public string Codigo { get; set; }
    }
}
