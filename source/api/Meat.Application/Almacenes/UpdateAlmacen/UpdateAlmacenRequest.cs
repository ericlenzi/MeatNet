using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.UpdateAlmacen
{
    public class UpdateAlmacenRequest : IRequest<UpdateAlmacenResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string TipoAlmacenId { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
