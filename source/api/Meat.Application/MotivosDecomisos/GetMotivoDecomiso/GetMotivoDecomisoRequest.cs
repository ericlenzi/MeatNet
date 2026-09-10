using MediatR;

namespace Meat.Application.MotivosDecomisos.GetMotivoDecomiso
{
    public class GetMotivoDecomisoRequest : IRequest<GetMotivoDecomisoResponse>
    {
        public string Codigo { get; set; }
    }
}
