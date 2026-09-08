using Meat.Domain.Especies;
using Meat.Domain.TiposSexos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.TiposEspecies
{
    public class TipoEspecie : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>Codigo de negocio, unico dentro de la empresa.</summary>
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public string TipoSexoId { get; set; }
        public virtual TipoSexo TipoSexo { get; set; }
        public string ERP_Codigo { get; set; }
        public double PesoTeorico { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
