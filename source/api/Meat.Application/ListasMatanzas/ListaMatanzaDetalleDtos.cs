using System;

namespace Meat.Application.ListasMatanzas
{
    /// <summary>Renglon de la Lista de Matanza (entrada desde el cliente).</summary>
    public class ListaMatanzaDetalleInput
    {
        public Guid TropaId { get; set; }
        public Guid AlmacenId { get; set; }
        public string TipoEspecieId { get; set; }
        public int Secuencia { get; set; }
        public int Cantidad { get; set; }
    }
}
