using Meat.Domain.Establecimientos;
using Meat.Domain.Usuarios;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.UsuariosEstablecimientos
{
    public class UsuarioEstablecimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public virtual Usuario Usuario { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public bool EsMain { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
