using Meat.Domain.Especies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.UnidadesFaenas
{
    /// <summary>
    /// Unidad de Faena por Especie (RES, MEDIA RES, CUARTO TRASERO, etc.).
    /// CantidadCuartos indica cuantos cuartos representa (ej. 1 RES = 4 cuartos; puede ser 0 para decomisos).
    /// PiezasPorAnimal = cuantas piezas de esta unidad forman un animal entero (RES=1, MEDIA RES=2,
    /// CUARTO=4). Es lo que define cuantas piezas se capturan por romaneo.
    /// PorDefecto marca la unidad predeterminada de la especie (una sola por especie).
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

        // Piezas de esta unidad que forman un animal entero (RES=1, MEDIA RES=2, CUARTO=4).
        public int PiezasPorAnimal { get; set; }

        // Unidad predeterminada de la especie (una sola por especie).
        public bool PorDefecto { get; set; }

        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
