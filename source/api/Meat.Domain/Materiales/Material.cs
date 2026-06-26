using Meat.Domain.Enums;
using Meat.Domain.TiposEmpresas;
using Meat.Domain.TiposMateriales;
using Meat.Domain.UnidadesMedidas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Materiales
{
    public class Material
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoMaterial { get; set; }
        public string Nombre { get; set; }
        public string TipoMaterialId { get; set; }
        public TipoMaterial TipoMaterial { get; set; }
        public string UnidadMedidaId { get; set; }
        public UnidadMedida UnidadMedida { get; set; }
        public string PesoTeorico { get; set; }
        public bool Activo { get; set; }
        public string ERP_Codigo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}