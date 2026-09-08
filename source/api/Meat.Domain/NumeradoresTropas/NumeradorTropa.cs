using Meat.Domain.ClientesEstablecimientos;
using Meat.Domain.Especies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.NumeradoresTropas
{
    public class NumeradorTropa : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ClienteEstablecimientoId { get; set; }
        public virtual ClienteEstablecimiento ClienteEstablecimiento { get; set; }

        public string EspecieCodigo { get; set; }
        public virtual Especie Especie { get; set; }

        public long UltimoNumeroTropa { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
