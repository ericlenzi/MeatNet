using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Tipificaciones.UpdateTipificacion
{
    public class UpdateTipificacionRequestFromBody
    {
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public Guid? TipoEspecieId { get; set; }
        public Guid? UnidadFaenaId { get; set; }
        public Guid? DestinoComercialId { get; set; }
        public string TipificacionOficialId { get; set; }
        public double PesoDesde { get; set; }
        public double PesoHasta { get; set; }
        public string UnidadMedidaId { get; set; }
        public Guid? MaterialId { get; set; }
        public bool Activo { get; set; }
    }
}
