using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Shared
{
    public class RequestBase
    {
        /// <summary>
        /// Empresa activa. La inyecta el controller desde el JWT: nunca se confia en el body,
        /// de ahi el JsonIgnore.
        /// </summary>
        [JsonIgnore]
        public string EmpresaId { get; set; }
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
