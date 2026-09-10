using MediatR;

namespace Meat.Application.Denticiones.UpdateDenticion
{
    public class UpdateDenticionRequest : IRequest<UpdateDenticionResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
