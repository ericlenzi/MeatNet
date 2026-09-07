using Meat.Domain.Almacenes;
using Meat.Domain.Especies;
using Meat.Domain.Materiales;
using Meat.Domain.Romaneos;
using Meat.Domain.TiposEspecies;
using Meat.Domain.TiposMovimientosCamaras;
using Meat.Domain.Tropas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.MovimientosCamaras
{
    /// <summary>
    /// Log append-only de la existencia de camara (Ciclo I paso 4). Es la fuente de verdad
    /// del stock de producto: el saldo por (Almacen, Material) se deriva sumando Cantidad y
    /// Peso, mismo patron que el En Pie derivado y que TropaMovimiento. Nunca se edita ni se
    /// borra; una correccion se hace con un contramovimiento.
    ///
    /// Nace en la Liberacion: cada RomaneoPieza no anulada genera un INGRESO (si su material
    /// no se cuartea) o un juego de TRANSF_BAJA + TRANSF_ALTA (si tiene despiece activo).
    /// </summary>
    public class MovimientoCamara
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid AlmacenId { get; set; }                    // camara destino; se valida Familia = CAMARA
        public virtual Almacen Almacen { get; set; }

        public Guid MaterialId { get; set; }                   // producto que entra o sale
        public virtual Material Material { get; set; }

        public string TipoMovimientoId { get; set; }           // ver TiposMovimientoCamara
        public virtual TipoMovimientoCamara TipoMovimiento { get; set; }

        public int Cantidad { get; set; }                      // piezas; positivo entra, negativo sale
        public double Peso { get; set; }                        // kg; mismo signo que Cantidad

        // Trazabilidad: de que media res / res salio este material.
        public Guid? RomaneoPiezaOrigenId { get; set; }
        public virtual RomaneoPieza RomaneoPiezaOrigen { get; set; }

        // Agrupa la BAJA y las ALTAS de un mismo cuarteo, para poder leerlo como una unidad.
        public Guid? TransformacionId { get; set; }

        // Trazabilidad denormalizada (evita joins largos al consultar existencia).
        public Guid? TropaId { get; set; }
        public virtual Tropa Tropa { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public DateTime Fecha { get; set; }
        public Guid? UsuarioId { get; set; }

        public string Referencia { get; set; }                  // texto legible del origen, ej. "Liberacion LM Nro 4"
    }
}
