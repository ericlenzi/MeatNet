using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Roles;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Usuarios
{
    public class Usuario : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; internal set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Legajo { get; set; }
        public string RolId { get; set; }
        public virtual Rol Rol { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public bool Activo { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}