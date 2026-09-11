using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Puestos.CreatePuesto
{
    public class CreatePuestoRequest : IRequest<CreatePuestoResponse>
    {
        [Required]
        public string CodigoPuesto { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public Guid EstablecimientoId { get; set; }
        [Required]
        public string EspecieId { get; set; }
        [Required]
        public string TipoPuestoId { get; set; }
        [Required]
        public string TipoMedicionId { get; set; }
    }
}
