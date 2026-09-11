using Meat.Domain.TiposMagnitudes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Medicion capturada de una pieza, tipada por el catalogo TiposMagnitudes (que se mide).
    /// En el MVP (Fase 2) la unica magnitud es PESO; la tabla queda para extender a
    /// Fase 2b (mas mediciones) sin cambiar el esquema.
    ///
    /// El metodo con el que se tomo el valor (manual, balanza, automatico) es de la cabecera del
    /// romaneo: Romaneo.TipoMedicionId.
    /// </summary>
    public class RomaneoPiezaMedicion : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid RomaneoPiezaId { get; set; }
        public virtual RomaneoPieza RomaneoPieza { get; set; }

        public string TipoMagnitudId { get; set; }
        public virtual TipoMagnitud TipoMagnitud { get; set; }

        public double Valor { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
