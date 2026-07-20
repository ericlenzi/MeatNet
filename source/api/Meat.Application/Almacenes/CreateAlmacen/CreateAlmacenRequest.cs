using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.CreateAlmacen
{
    public class CreateAlmacenRequest : IRequest<CreateAlmacenResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        [Required]
        public string CodigoAlmacen { get; set; }
        [Required]
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string TipoAlmacenId { get; set; }
        public string ERP_Codigo { get; set; }
        public Guid EstablecimientoId { get; set; }
    }
}
