using Meat.Domain.DestinosComerciales;
using Meat.Domain.Especies;
using Meat.Domain.TipificacionesOficiales;
using Meat.Domain.TiposEspecies;
using Meat.Domain.UnidadesFaenas;
using Meat.Domain.UnidadesMedidas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Tipificaciones
{
    /// <summary>
    /// Tipificacion parametrizable por Empresa. Clasifica la media res segun especie,
    /// categoria, unidad de faena, destino, tipificacion oficial y rango de peso.
    /// Puntos se incrementa cada vez que se usa (para ordenar las mas frecuentes primero).
    /// </summary>
    public class Tipificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        // Empresa activa (columna simple, no FK: el sistema opera para una unica Empresa).
        public string CodigoEmpresa { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public string UnidadFaenaId { get; set; }              // FK a UnidadFaena.Codigo
        public virtual UnidadFaena UnidadFaena { get; set; }

        public string DestinoComercialId { get; set; }
        public virtual DestinoComercial DestinoComercial { get; set; }

        public string TipificacionOficialId { get; set; }
        public virtual TipificacionOficial TipificacionOficial { get; set; }

        public double PesoDesde { get; set; }
        public double PesoHasta { get; set; }

        public string UnidadMedidaId { get; set; }
        public virtual UnidadMedida UnidadMedida { get; set; }

        public int Puntos { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
