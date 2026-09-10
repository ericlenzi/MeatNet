using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.EvaluacionFaena.LiberarJornada
{
    public class LiberarJornadaRequest : RequestBase, IRequest<LiberarJornadaResponse>
    {

        public Guid ListaMatanzaId { get; set; }
    }

    public class LiberarJornadaResponse
    {
        /// <summary>Movimientos de camara escritos en el log.</summary>
        public int MovimientosGenerados { get; set; }

        /// <summary>Piezas que pasaron a Liberado en esta corrida.</summary>
        public int PiezasLiberadas { get; set; }

        /// <summary>Romaneos que quedaron definitivos en esta corrida.</summary>
        public int RomaneosLiberados { get; set; }

        /// <summary>Piezas que ya estaban liberadas y se saltearon (R-L7).</summary>
        public int PiezasYaLiberadas { get; set; }

        /// <summary>Kilos netos que quedaron en camara (el cuarteo no duplica: la media res se cancela con su baja).</summary>
        public double KilosIngresados { get; set; }

        /// <summary>Piezas de reses condenadas (R-E23): se fijaron sin generar existencia.</summary>
        public int PiezasDecomisadas { get; set; }

        /// <summary>Kilos condenados que no entraron a camara.</summary>
        public double KilosDecomisados { get; set; }
    }
}
