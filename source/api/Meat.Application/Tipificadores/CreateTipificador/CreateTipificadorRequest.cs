using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Tipificadores.CreateTipificador
{
    public class CreateTipificadorRequest : IRequest<CreateTipificadorResponse>
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Matricula { get; set; }
        [Required]
        public Guid EstablecimientoId { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public bool PorDefecto { get; set; }
    }
}
