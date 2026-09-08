using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Tipificaciones.UpdateTipificacion
{
    public class UpdateTipificacionRequest : IRequest<UpdateTipificacionResponse>
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonIgnore]
        public string EmpresaId { get; set; }

        public string Descripcion { get; set; }
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
