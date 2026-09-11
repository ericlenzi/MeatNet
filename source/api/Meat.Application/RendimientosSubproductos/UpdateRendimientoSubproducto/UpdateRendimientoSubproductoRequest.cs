using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.RendimientosSubproductos.UpdateRendimientoSubproducto
{
    public class UpdateRendimientoSubproductoRequest : IRequest<UpdateRendimientoSubproductoResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string EspecieId { get; set; }
        public Guid MaterialId { get; set; }
        public double Porcentaje { get; set; }
        public bool Activo { get; set; }
    }
}
