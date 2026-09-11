using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposMediciones
{
    /// <summary>
    /// Catalogo: como se toma la medicion en el puesto (M manual, B balanza, A automatica).
    /// Es el metodo de captura, no la magnitud: que se mide lo dice TipoMagnitud (PESO).
    ///
    /// Cada Puesto declara con cual mide por defecto, y el Tipificador lo propone en la cabecera
    /// del romaneo; el operario puede cambiarlo si ese dia pesa de otra forma.
    /// </summary>
    public class TipoMedicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
