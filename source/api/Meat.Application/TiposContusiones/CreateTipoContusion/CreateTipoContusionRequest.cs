using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposContusiones.CreateTipoContusion
{
    public class CreateTipoContusionRequest : IRequest<CreateTipoContusionResponse>
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
