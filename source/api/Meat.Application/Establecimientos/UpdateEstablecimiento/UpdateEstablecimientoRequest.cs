using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoRequest : IRequest<UpdateEstablecimientoResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public IEnumerable<string> EspecieIds { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroRuca { get; set; }
        public bool Activo { get; set; }
    }
}
