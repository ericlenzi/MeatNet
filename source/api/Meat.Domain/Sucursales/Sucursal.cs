using Meat.Domain.Empresas;
using Meat.Domain.Puestos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Sucursales
{
    public class Sucursal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoSucursal { get; set; }
        public string Nombre { get; set; }
        public string Erp_Codigo { get; set; }
        public Guid EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Color { get; set; }
        public virtual ICollection<Puesto> Puestos { get; set; }
    }
}