using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.Romaneos.CrearRomaneo
{
    public class CrearRomaneoRequest : IRequest<CrearRomaneoResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public Guid ListaMatanzaId { get; set; }
        public Guid ListaMatanzaDetalleId { get; set; }     // renglon elegido (hibrido)
        public string UnidadFaenaId { get; set; }           // FK a UnidadFaena.Codigo
        public int NumeroGarron { get; set; }

        public List<PiezaRomaneoInput> Piezas { get; set; } = new List<PiezaRomaneoInput>();
    }

    public class PiezaRomaneoInput
    {
        // La letra la asigna el servidor (A/B/... para vacuno; null para porcino).
        public Guid AlmacenDestinoId { get; set; }          // camara destino de la pieza (default del renglon; obligatoria)
        public string TipificacionId { get; set; }
        public double Peso { get; set; }
        public bool ForzarFueraRango { get; set; }          // confirmacion explicita del operario para pesar fuera del rango de la tipificacion
    }

    public class CrearRomaneoResponse
    {
        public Guid Id { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
    }
}
