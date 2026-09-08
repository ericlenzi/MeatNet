using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoRequest : RequestBase, IRequest<CreateEstablecimientoResponse>
    {
        [Required]
        public string CodigoEstablecimiento { get; set; }

        [Required]
        public string Nombre { get; set; }

        public Guid SucursalId { get; set; }
        public IEnumerable<string> EspecieIds { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroRuca { get; set; }
    }
}
