using MediatR;

namespace Meat.Application.Conformaciones.UpdateConformacion
{
    public class UpdateConformacionRequest : IRequest<UpdateConformacionResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
