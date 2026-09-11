namespace Meat.Application.Establecimientos
{
    public class EspecieItem
    {
        public string Id { get; set; }
        public string Nombre { get; set; }

        /// <summary>Merma de oreo de esta planta para esta especie (%), si la declaro (R-A8).</summary>
        public double? MermaOreo { get; set; }

        /// <summary>La del catalogo de la especie: es lo que vale cuando la planta no declara una.</summary>
        public double? MermaOreoReferencia { get; set; }
    }
}
