using Meat.Domain.Sucursales;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Puestos
{
    public class Puesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoPuesto { get; set; }
        public string Nombre { get; set; }
        public string Erp_Codigo { get; set; }
        public Guid SucursalId { get; set; }
        public virtual Sucursal Sucursal { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; }
    }
}