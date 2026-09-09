using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.Romaneos.CrearRomaneo
{
    public class CrearRomaneoRequest : RequestBase, IRequest<CrearRomaneoResponse>
    {

        public Guid ListaMatanzaId { get; set; }
        public Guid ListaMatanzaDetalleId { get; set; }     // renglon elegido (hibrido)
        public Guid? UnidadFaenaId { get; set; }
        public int NumeroGarron { get; set; }

        // Ejes de la tipificacion oficial que se determinan mirando la res. Opcionales: no toda
        // especie tiene tipificacion oficial cargada.
        public string ConformacionId { get; set; }
        public string GradoEngrasamientoId { get; set; }

        public List<PiezaRomaneoInput> Piezas { get; set; } = new List<PiezaRomaneoInput>();
    }

    public class PiezaRomaneoInput
    {
        // La letra la asigna el servidor (A/B/... para vacuno; null para porcino).
        public Guid AlmacenDestinoId { get; set; }          // camara destino de la pieza (default del renglon; obligatoria)
        public Guid? TipificacionId { get; set; }
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
