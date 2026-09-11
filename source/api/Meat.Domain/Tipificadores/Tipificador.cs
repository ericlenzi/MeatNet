using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Tipificadores
{
    /// <summary>
    /// Persona habilitada para tipificar en el palco. Se configura por Empresa +
    /// Establecimiento + Especie, igual que el Puesto: la matricula habilita para una especie.
    ///
    /// PorDefecto marca el que el Tipificador propone en la cabecera del romaneo (uno solo por
    /// establecimiento y especie). El romaneo guarda quien tipifico, asi que el dato es
    /// historico y un tipificador que ya tipifico no se elimina: se desactiva.
    /// </summary>
    public class Tipificador : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Nombre { get; set; }

        /// <summary>Matricula del tipificador, unica dentro de la empresa.</summary>
        public string Matricula { get; set; }

        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Tipificador propuesto por defecto (uno solo por Establecimiento + Especie).</summary>
        public bool PorDefecto { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
