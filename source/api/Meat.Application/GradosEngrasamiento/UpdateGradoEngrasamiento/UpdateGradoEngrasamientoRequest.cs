using MediatR;

namespace Meat.Application.GradosEngrasamiento.UpdateGradoEngrasamiento
{
    public class UpdateGradoEngrasamientoRequest : IRequest<UpdateGradoEngrasamientoResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
