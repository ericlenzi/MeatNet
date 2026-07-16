using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.AgregarRenglonListaMatanza
{
    public class AgregarRenglonListaMatanzaRequest : IRequest<AgregarRenglonListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }                       // lista de matanza
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public Guid TropaId { get; set; }
        public Guid AlmacenId { get; set; }
        public string TipoEspecieId { get; set; }
        public int Cantidad { get; set; }
        public int? Secuencia { get; set; }                // null = al final
        public string Motivo { get; set; }
    }

    public class AgregarRenglonListaMatanzaResponse
    {
        public Guid RenglonId { get; set; }
    }
}
