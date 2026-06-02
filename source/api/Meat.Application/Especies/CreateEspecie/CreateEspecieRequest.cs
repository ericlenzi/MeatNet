using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Especies.CreateEspecie
{
    public class CreateEspecieRequest : IRequest<CreateEspecieResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}
