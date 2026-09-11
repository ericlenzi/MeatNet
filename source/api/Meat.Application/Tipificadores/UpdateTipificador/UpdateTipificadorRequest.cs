using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Tipificadores.UpdateTipificador
{
    public class UpdateTipificadorRequest : IRequest<UpdateTipificadorResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EspecieId { get; set; }
        public bool PorDefecto { get; set; }
        public bool Activo { get; set; }
    }
}
