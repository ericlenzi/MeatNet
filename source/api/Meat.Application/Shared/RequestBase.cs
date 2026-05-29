using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Shared
{
    public class RequestBase
    {
        public string CodigoEmpresa { get; set; }
        //[JsonIgnore]
        public string NumeroSucursal { get; set; }
        //[JsonIgnore]
        public string NumeroPuesto { get; set; }
        //[JsonIgnore]
        public Guid UsuarioId { get; set; }
        [JsonIgnore]
        public bool IsTransaction { get; set; }
    }
}
