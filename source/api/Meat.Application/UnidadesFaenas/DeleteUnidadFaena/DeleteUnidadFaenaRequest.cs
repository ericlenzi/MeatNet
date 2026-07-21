using MediatR;

namespace Meat.Application.UnidadesFaenas.DeleteUnidadFaena
{
    public class DeleteUnidadFaenaRequest : IRequest<DeleteUnidadFaenaResponse>
    {
        public string Codigo { get; set; }
    }
}
