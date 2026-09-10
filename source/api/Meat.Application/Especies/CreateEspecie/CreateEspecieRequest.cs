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

        /// <summary>Banda de rinde esperable (%), para el aviso del Analisis (R-A7).</summary>
        public double? RindeMinimo { get; set; }
        public double? RindeMaximo { get; set; }
    }
}
