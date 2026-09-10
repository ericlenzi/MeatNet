using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.EvaluacionFaena.GetRomaneosEvaluacion
{
    public class GetRomaneosEvaluacionRequest : RequestBase, IRequest<GetRomaneosEvaluacionResponse>
    {

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetRomaneosEvaluacionResponse
    {
        // Cabecera de la jornada, para encabezar la pantalla y la planilla impresa.
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public DateTime Fecha { get; set; }
        public string EspecieId { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        public string EstablecimientoNombre { get; set; }

        public int TotalRomaneos { get; set; }
        public int TotalPiezas { get; set; }
        public double TotalKg { get; set; }
        public int PiezasLiberadas { get; set; }

        /// <summary>Reses condenadas enteras de la jornada (R-E23), sin contar anuladas.</summary>
        public int TotalDecomisosTotales { get; set; }

        /// <summary>Medias reses condenadas por separado (R-E27), sin contar anuladas.</summary>
        public int TotalPiezasDecomisadas { get; set; }

        /// <summary>Kilos condenados: los de las reses condenadas mas los recortes parciales.</summary>
        public double TotalKgDecomisados { get; set; }

        public IEnumerable<RomaneoEvaluacionItem> Data { get; set; } = new List<RomaneoEvaluacionItem>();

        /// <summary>Camaras del establecimiento, para el selector al editar una pieza.</summary>
        public IEnumerable<CamaraOpcion> Camaras { get; set; } = new List<CamaraOpcion>();
    }

    public class RomaneoEvaluacionItem
    {
        public Guid Id { get; set; }
        public long NumeroRomaneo { get; set; }
        public int NumeroGarron { get; set; }
        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public string UnidadFaenaNombre { get; set; }
        public DateTime Fecha { get; set; }

        public bool Anulado { get; set; }
        public bool Liberado { get; set; }
        public DateTime? FechaLiberacion { get; set; }

        // Res condenada entera (R-E23): se pesa y se libera, pero no entra a camara.
        public bool DecomisoTotal { get; set; }
        public string MotivoDecomisoNombre { get; set; }

        public double PesoTotal { get; set; }

        public IEnumerable<PiezaEvaluacionItem> Piezas { get; set; } = new List<PiezaEvaluacionItem>();
    }

    public class PiezaEvaluacionItem
    {
        public Guid Id { get; set; }
        public string Letra { get; set; }
        public double Peso { get; set; }
        public bool PesoFueraRango { get; set; }
        public bool Liberado { get; set; }

        public Guid AlmacenDestinoId { get; set; }
        public string AlmacenDestinoNombre { get; set; }

        public Guid? TipificacionId { get; set; }
        public string TipificacionDescripcion { get; set; }

        // Material que produciria esta pieza al liberar. Vacio = la liberacion se va a bloquear,
        // salvo que la res este condenada: esas piezas no producen material a proposito.
        public Guid? MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }

        // Decomiso de esta media res: condenada entera (R-E27) o recorte de kilos (R-E24).
        public bool Decomisada { get; set; }
        public string MotivoDecomisoNombre { get; set; }
        public double PesoDecomisado { get; set; }
    }

    public class CamaraOpcion
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
    }
}
