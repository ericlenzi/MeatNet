using Meat.Domain.Especies;
using Meat.Domain.TiposSexos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposEspecies
{
    public class TipoEspecie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public string TipoSexoId { get; set; }
        public virtual TipoSexo TipoSexo { get; set; }
        public string CodigoMaterialDesde { get; set; }
        public string CodigoMaterialHasta { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
