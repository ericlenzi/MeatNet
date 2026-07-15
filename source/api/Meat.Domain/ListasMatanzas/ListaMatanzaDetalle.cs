using Meat.Domain.Almacenes;
using Meat.Domain.Tropas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.ListasMatanzas
{
    /// <summary>
    /// Renglon de la Lista de Matanza: una Tropa, en un Corral, con cantidad y
    /// secuencia de faena. Una misma (Tropa, Corral) puede aparecer en varios
    /// renglones (resultado de "dividir"); la secuencia solo ordena, no es clave.
    /// </summary>
    public class ListaMatanzaDetalle
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ListaMatanzaId { get; set; }
        public virtual ListaMatanza ListaMatanza { get; set; }

        public Guid TropaId { get; set; }
        public virtual Tropa Tropa { get; set; }

        public Guid AlmacenId { get; set; }                // corral de origen
        public virtual Almacen Almacen { get; set; }

        public int Secuencia { get; set; }                 // orden de faena (reordenable)
        public int Cantidad { get; set; }                  // animales a faenar de esta tropa/corral
        public int CantidadFaenada { get; set; }           // lo actualiza el Monitor; congela el renglon
    }
}
