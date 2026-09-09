using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.GradosEngrasamiento.CreateGradoEngrasamiento
{
    public class CreateGradoEngrasamientoRequest : IRequest<CreateGradoEngrasamientoResponse>
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
