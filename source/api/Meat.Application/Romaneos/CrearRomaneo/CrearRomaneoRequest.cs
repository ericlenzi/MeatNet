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

        // Cabecera del puesto. El tipificador es obligatorio cuando el establecimiento tiene
        // tipificadores cargados para la especie (misma regla derivada del catalogo que los
        // datos del palco). El metodo de medicion, si no viene, sale del puesto de la lista.
        public Guid? TipificadorId { get; set; }
        public string TipoMedicionId { get; set; }

        // Datos del palco que se determinan mirando la res, no el animal en pie. Van sin
        // [Required]: si son obligatorios o no lo decide la especie de la jornada, y eso solo se
        // sabe consultando el catalogo (R-E20).
        public string ConformacionId { get; set; }
        public string GradoEngrasamientoId { get; set; }
        public string DenticionId { get; set; }

        // Decomiso total (R-E23): la inspeccion condena la res entera. El animal se faena igual,
        // sus piezas se pesan y el motivo pasa a ser obligatorio; a cambio, la res condenada no
        // se clasifica (ni datos del palco ni tipificacion) porque no va a ser carne.
        public bool DecomisoTotal { get; set; }
        public string MotivoDecomisoId { get; set; }

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

        // Media res condenada entera (R-E27): esta pieza no va a camara y la otra del animal
        // sigue su curso. Exige motivo y deja sin pedir tipificacion ni contusion de la pieza.
        public bool Decomisada { get; set; }

        // Motivo del decomiso de la pieza, sea la condena entera (R-E27) o el recorte parcial
        // (R-E24). En el recorte van juntos motivo y kilos, y no descuentan el Peso: la pieza
        // sigue a camara y los kilos se informan como merma sanitaria.
        public string MotivoDecomisoId { get; set; }
        public double PesoDecomisado { get; set; }
    }

    public class CrearRomaneoResponse
    {
        public Guid Id { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
    }
}
