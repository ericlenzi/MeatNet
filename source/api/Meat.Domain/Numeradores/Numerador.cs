using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Numeradores
{
    /// <summary>
    /// Numerador generico por Establecimiento + Especie + TipoNumerador.
    /// Ej. TipoNumerador = "ROMANEO" lleva la secuencia de los romaneos creados.
    /// </summary>
    public class Numerador : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        public string EspecieCodigo { get; set; }
        public virtual Especie Especie { get; set; }

        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string TipoNumerador { get; set; }
        public int UltimoNumero { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
