using Meat.Domain.Empresas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Roles
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public Guid EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
