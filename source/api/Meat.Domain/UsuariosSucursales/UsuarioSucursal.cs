using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Roles;
using Meat.Domain.Usuarios;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.UsuariosSucursales
{
    public class UsuarioSucursal : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public Guid SucursalId { get; set; }
        public bool EsMain { get; set; }
        public DateTime FechaActualizacion { get; set; }

        //ToDo: roles por sucursal
        //public string RolId { get; set; }
        //public virtual Rol Rol { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
