using MediatR;

namespace Meat.Application.TiposPuestos.DeleteTipoPuesto
{
    public class DeleteTipoPuestoRequest : IRequest<DeleteTipoPuestoResponse>
    {
        public string Codigo { get; set; }
    }
}
