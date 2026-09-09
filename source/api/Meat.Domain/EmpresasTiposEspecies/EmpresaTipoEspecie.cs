using Meat.Domain.Empresas;
using Meat.Domain.Shared;
using Meat.Domain.TiposEspecies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.EmpresasTiposEspecies
{
    /// <summary>
    /// Configuracion de una empresa sobre una categoria del catalogo global TipoEspecie.
    ///
    /// La fila existe si la empresa opera con esa categoria: sin fila, la categoria no aparece
    /// en sus pantallas. Lleva los parametros que si son propios de cada planta, y el
    /// PesoTeorico de aca (no el de referencia del catalogo) es el que leen los calculos.
    /// </summary>
    public class EmpresaTipoEspecie : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        /// <summary>Peso teorico en kg con el que trabaja esta empresa. Se propone desde
        /// TipoEspecie.PesoTeoricoReferencia en el alta y despues queda desacoplado.</summary>
        public double PesoTeorico { get; set; }

        /// <summary>Codigo del articulo equivalente en el ERP de la empresa.</summary>
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
