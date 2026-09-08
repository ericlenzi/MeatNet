using Meat.Domain.Empresas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Shared;

namespace Meat.Domain.Parametros
{
    public class Parametro : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>Codigo de negocio, unico dentro de la empresa.</summary>
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool Activo { get; set; }
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
