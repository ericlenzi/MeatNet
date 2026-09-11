using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.EstablecimientosEspecies
{
    public class EstablecimientoEspecie : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>
        /// Merma de oreo que observa ESTA planta para ESTA especie, en porcentaje. Depende de la
        /// camara, del tiempo de oreo y de la cobertura de grasa, asi que dos plantas de la misma
        /// empresa pueden tener numeros distintos.
        ///
        /// Nullable: si no esta cargada vale la referencia de la especie, y si esa tampoco esta,
        /// el Analisis no muestra rinde frio (R-A8). Mismo criterio que el peso teorico de las
        /// categorias, donde el catalogo propone y cada empresa ajusta.
        /// </summary>
        public double? MermaOreo { get; set; }

        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
