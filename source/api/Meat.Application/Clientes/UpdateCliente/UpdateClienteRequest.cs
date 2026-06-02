using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Clientes.UpdateCliente
{
    public class UpdateClienteRequest : IRequest<UpdateClienteResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public string TipoClienteId { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
