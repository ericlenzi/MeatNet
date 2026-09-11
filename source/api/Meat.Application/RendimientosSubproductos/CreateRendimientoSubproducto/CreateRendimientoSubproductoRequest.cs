using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.RendimientosSubproductos.CreateRendimientoSubproducto
{
    public class CreateRendimientoSubproductoRequest : IRequest<CreateRendimientoSubproductoResponse>
    {
        [Required]
        public string EspecieId { get; set; }
        [Required]
        public Guid MaterialId { get; set; }
        public double Porcentaje { get; set; }
    }
}
