using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Establecimientos.CreateEstablecimiento
{
    public class CreateEstablecimientoRequest : IRequest<CreateEstablecimientoResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
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
