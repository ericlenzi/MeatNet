using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposAlmacenes
{
    public class TipoAlmacen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        /// <summary>Familia del tipo de almacen: CORRAL (recepcion, en pie) o CAMARA (faena, media res).</summary>
        public string Familia { get; set; }
        public bool Activo { get; set; }
    }
}