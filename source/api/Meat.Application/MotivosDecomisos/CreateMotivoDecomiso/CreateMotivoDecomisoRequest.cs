using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.MotivosDecomisos.CreateMotivoDecomiso
{
    public class CreateMotivoDecomisoRequest : IRequest<CreateMotivoDecomisoResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public int Orden { get; set; }

        /// <summary>El motivo describe un golpe: exige contusion en la pieza (R-E26).</summary>
        public bool ExigeContusion { get; set; }
    }
}
