using MediatR;

namespace Meat.Application.MotivosDecomisos.DeleteMotivoDecomiso
{
    public class DeleteMotivoDecomisoRequest : IRequest<DeleteMotivoDecomisoResponse>
    {
        public string Codigo { get; set; }
    }
}
