using Meat.Domain.Especies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.UnidadesFaenas
{
    /// <summary>
    /// Unidad de Faena por Especie (RES, MEDIA RES, CUARTO TRASERO, etc.).
    /// CantidadCuartos indica cuantos cuartos representa (ej. 1 RES = 4 cuartos; puede ser 0 para decomisos).
    /// UnidadComplementaria referencia la unidad que la completa (ej. MEDIA RES se complementa consigo misma).
    /// </summary>
    public class UnidadFaena
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public int Numero { get; set; }
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }

        // Cantidad de unidades complementarias para completar los cuartos (ej. MEDIA RES = 2).
        public int UnidadComplementaria { get; set; }

        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
