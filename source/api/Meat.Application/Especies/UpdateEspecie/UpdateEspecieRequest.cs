using MediatR;

namespace Meat.Application.Especies.UpdateEspecie
{
    public class UpdateEspecieRequest : IRequest<UpdateEspecieResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
