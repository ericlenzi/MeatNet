using MediatR;

namespace Meat.Application.TiposPuestos.GetTipoPuesto
{
    public class GetTipoPuestoRequest : IRequest<GetTipoPuestoResponse>
    {
        public string Codigo { get; set; }
    }
}
