using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.UpdateAlmacen
{
    public class UpdateAlmacenRequest : RequestBase, IRequest<UpdateAlmacenResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }


        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string TipoAlmacenId { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
