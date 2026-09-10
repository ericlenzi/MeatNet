using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.Romaneos.GetRomaneosJornada
{
    public class GetRomaneosJornadaRequest : RequestBase, IRequest<GetRomaneosJornadaResponse>
    {

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetRomaneosJornadaResponse
    {
        public IEnumerable<RomaneoJornadaItem> Data { get; set; } = new List<RomaneoJornadaItem>();
    }

    public class RomaneoJornadaItem
    {
        public Guid Id { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public long NumeroTropa { get; set; }
        public string TipoEspecieNombre { get; set; }
        public string UnidadFaenaNombre { get; set; }
        public bool Anulado { get; set; }
        public DateTime Fecha { get; set; }
        public double PesoTotal { get; set; }
        public IEnumerable<RomaneoPiezaItem> Piezas { get; set; } = new List<RomaneoPiezaItem>();
    }

    public class RomaneoPiezaItem
    {
        public string Letra { get; set; }
        public double Peso { get; set; }
        public string AlmacenDestinoNombre { get; set; }
        public Guid? TipificacionId { get; set; }
        public string TipificacionDescripcion { get; set; }
        public string TipoContusionNombre { get; set; }
        public bool PesoFueraRango { get; set; }
    }
}
