using MediatR;

namespace Meat.Application.MotivosDecomisos.UpdateMotivoDecomiso
{
    public class UpdateMotivoDecomisoRequest : IRequest<UpdateMotivoDecomisoResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
