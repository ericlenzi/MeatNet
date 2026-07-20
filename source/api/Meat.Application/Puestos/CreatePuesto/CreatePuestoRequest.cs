using MediatR;
using System.ComponentModel.DataAnnotations;
using System;

namespace Meat.Application.Puestos.CreatePuesto
{
    public class CreatePuestoRequest : IRequest<CreatePuestoResponse>
    {
        [Required]
        public string NumeroPuesto { get; set; }
        [Required]
        public Guid EstablecimientoId { get; set; }
        public string Erp_Codigo { get; set; }
    }
}
