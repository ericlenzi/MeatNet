using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Denticiones.CreateDenticion
{
    public class CreateDenticionRequest : IRequest<CreateDenticionResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public int Orden { get; set; }
    }
}
