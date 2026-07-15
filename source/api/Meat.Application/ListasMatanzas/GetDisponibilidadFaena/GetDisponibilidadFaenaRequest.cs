using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.GetDisponibilidadFaena
{
    public class GetDisponibilidadFaenaRequest : IRequest<GetDisponibilidadFaenaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid EstablecimientoId { get; set; }
        public string EspecieId { get; set; }
        public Guid? ExcludeListaId { get; set; }   // LM en edicion, para no restar su propia reserva
    }

    public class GetDisponibilidadFaenaResponse
    {
        public IEnumerable<DisponibilidadFaenaItem> Data { get; set; } = new List<DisponibilidadFaenaItem>();
    }

    public class DisponibilidadFaenaItem
    {
        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public int EnPie { get; set; }
        public int Reservado { get; set; }
        public int Disponible { get; set; }
        public double PesoPromedio { get; set; }
    }
}
