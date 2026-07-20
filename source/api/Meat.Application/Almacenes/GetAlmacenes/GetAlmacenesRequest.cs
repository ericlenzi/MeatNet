using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Almacenes.GetAlmacenes
{
    public class GetAlmacenesRequest : IRequest<GetAlmacenesResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }

        /// <summary>Filtra por estado activo/inactivo. Si es null, devuelve todos.</summary>
        public bool? Estado { get; set; }

        /// <summary>Filtra por familia del tipo de almacen (CORRAL / CAMARA). Si es null, devuelve todas.</summary>
        public string Familia { get; set; }
    }
}
