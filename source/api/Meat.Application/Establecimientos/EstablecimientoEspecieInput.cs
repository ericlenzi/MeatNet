using System.Collections.Generic;

namespace Meat.Application.Establecimientos
{
    /// <summary>
    /// Especie habilitada en un establecimiento, con el parametro que la planta le cuelga.
    /// Viaja asi y no como una lista de codigos porque la merma de oreo es por planta y especie
    /// (R-A8): mandar solo los codigos borraria el valor cargado en cada guardado.
    /// </summary>
    public class EstablecimientoEspecieInput
    {
        public string EspecieId { get; set; }

        /// <summary>Merma de oreo de la planta (%). Null = vale la referencia de la especie.</summary>
        public double? MermaOreo { get; set; }
    }
}
