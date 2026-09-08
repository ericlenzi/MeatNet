using Meat.Application.Shared;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Almacenes.CreateAlmacen
{
    public class CreateAlmacenRequest : RequestBase, IRequest<CreateAlmacenResponse>
    {

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
