using Meat.Domain.Empresas;
using Meat.Domain.Enums;
using Meat.Domain.Especies;
using Meat.Domain.Sucursales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Establecimientos
{
    public class Establecimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public virtual Sucursal Sucursal { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroOncca { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public Guid EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}