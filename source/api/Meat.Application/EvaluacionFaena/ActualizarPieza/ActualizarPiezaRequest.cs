using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.EvaluacionFaena.ActualizarPieza
{
    public class ActualizarPiezaRequest : IRequest<ActualizarPiezaResponse>
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public Guid Id { get; set; }

        public double Peso { get; set; }
        public Guid? TipificacionId { get; set; }
        public Guid AlmacenDestinoId { get; set; }

        /// <summary>
        /// Confirmacion explicita para dejar el peso fuera del rango de la tipificacion, igual
        /// que en el Tipificador. Sin esto, un peso fuera de rango se rechaza.
        /// </summary>
        public bool ForzarFueraRango { get; set; }
    }

    /// <summary>Cuerpo del PUT: el Id viaja en la ruta.</summary>
    public class ActualizarPiezaBody
    {
        public double Peso { get; set; }
        public Guid? TipificacionId { get; set; }
        public Guid AlmacenDestinoId { get; set; }
        public bool ForzarFueraRango { get; set; }
    }

    public class ActualizarPiezaResponse
    {
        public Guid Id { get; set; }
        public double Peso { get; set; }
        public bool PesoFueraRango { get; set; }
        public Guid? TipificacionId { get; set; }
        public Guid AlmacenDestinoId { get; set; }
    }
}
