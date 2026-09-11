using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using Meat.Domain.TiposMediciones;
using Meat.Domain.TiposPuestos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Puestos
{
    /// <summary>
    /// Puesto de la planta (el palco de faena, por ahora el unico tipo). Se configura por
    /// Empresa + Establecimiento + Especie: un palco vacuno y un palco porcino son dos puestos,
    /// porque la linea, la gente y la forma de medir son distintas.
    ///
    /// Cada Lista de Matanza declara en que puesto se va a faenar, y el Tipificador solo ve las
    /// listas asignadas al puesto desde el que se esta trabajando.
    /// </summary>
    public class Puesto : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>Codigo de negocio, unico dentro de la empresa.</summary>
        public string CodigoPuesto { get; set; }
        public string Nombre { get; set; }

        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        /// <summary>Especie con la que opera el puesto (el palco vacuno no es el palco porcino).</summary>
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public string TipoPuestoId { get; set; }
        public virtual TipoPuesto TipoPuesto { get; set; }

        /// <summary>
        /// Metodo con el que este puesto mide por defecto (manual, balanza, automatico). Es el
        /// valor que el Tipificador propone en la cabecera del romaneo.
        /// </summary>
        public string TipoMedicionId { get; set; }
        public virtual TipoMedicion TipoMedicion { get; set; }

        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
