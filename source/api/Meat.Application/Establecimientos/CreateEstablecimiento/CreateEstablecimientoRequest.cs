using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoRequest : IRequest<CreateEstablecimientoResponse>
    {
        [Required]
        public string CodigoEstablecimiento { get; set; }

        [Required]
        public string Nombre { get; set; }

        public Guid SucursalId { get; set; }
        public string EspecieId { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroOncca { get; set; }
    }
}
