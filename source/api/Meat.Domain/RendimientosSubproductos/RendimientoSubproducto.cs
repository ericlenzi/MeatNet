using Meat.Domain.Especies;
using Meat.Domain.Materiales;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.RendimientosSubproductos
{
    /// <summary>
    /// Cuanto subproducto deja un animal, en porcentaje del peso de la res faenada: cuero, sebo,
    /// menudencias. Es un parametro de estimacion, no una medicion.
    ///
    /// No es un DespieceMaterial y no puede serlo: el despiece reparte el peso de un material
    /// entre sus destinos y la suma tiene que cerrar en 100%, que es lo que hace que el cuarteo
    /// controle masa. El cuero nunca estuvo en el peso de la media res, asi que colgarlo ahi
    /// inflaria o desinflaria la carne para hacerle lugar.
    ///
    /// La base es el peso de la res faenada y no el peso vivo, porque el vivo es justo el dato
    /// que a veces falta (R-A3) y el de faena esta siempre y es medido.
    /// </summary>
    public class RendimientoSubproducto : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>El subproducto que se estima. Su TipoMaterial tiene que ser de subproducto.</summary>
        public Guid MaterialId { get; set; }
        public virtual Material Material { get; set; }

        /// <summary>Porcentaje sobre los kg de res faenada. Mayor a 0 y menor a 100.</summary>
        public double Porcentaje { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
