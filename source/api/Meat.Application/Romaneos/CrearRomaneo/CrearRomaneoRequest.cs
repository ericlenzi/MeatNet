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

        // Datos del palco que se determinan mirando la res, no el animal en pie. Van sin
        // [Required]: si son obligatorios o no lo decide la especie de la jornada, y eso solo se
        // sabe consultando el catalogo (R-E20).
        public string ConformacionId { get; set; }
        public string GradoEngrasamientoId { get; set; }
        public string DenticionId { get; set; }

        public List<PiezaRomaneoInput> Piezas { get; set; } = new List<PiezaRomaneoInput>();
    }

    public class PiezaRomaneoInput
    {
        // La letra la asigna el servidor (A/B/... para vacuno; null para porcino).
        public Guid AlmacenDestinoId { get; set; }          // camara destino de la pieza (default del renglon; obligatoria)
        public Guid? TipificacionId { get; set; }
        public string TipoContusionId { get; set; }         // contusion de esta media res; el golpe es de la pieza, no del animal
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
