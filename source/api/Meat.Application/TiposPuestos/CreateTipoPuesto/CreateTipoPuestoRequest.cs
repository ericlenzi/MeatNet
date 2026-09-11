using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposPuestos.CreateTipoPuesto
{
    public class CreateTipoPuestoRequest : IRequest<CreateTipoPuestoResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}
