using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.UpdateIngresoHacienda
{
    public class UpdateIngresoHaciendaRequest : IRequest<UpdateIngresoHaciendaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public DateTime FechaHoraIngreso { get; set; }

        public string EspecieId { get; set; }

        public string NumeroDte { get; set; }
        public DateTime FechaEmisionDte { get; set; }

        public Guid ClienteId { get; set; }
        public Guid ClienteEstablecimientoId { get; set; }

        public int ProvinciaId { get; set; }
        public string Localidad { get; set; }

        public string OrigenHaciendaId { get; set; }
        public string UsoHaciendaId { get; set; }

        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string PatenteCamion { get; set; }
        public string PatenteJaula { get; set; }

        public List<IngresoHaciendaPesadaInput> Pesadas { get; set; } = new List<IngresoHaciendaPesadaInput>();
        public List<IngresoHaciendaUbicacionInput> Ubicaciones { get; set; } = new List<IngresoHaciendaUbicacionInput>();
    }
}
