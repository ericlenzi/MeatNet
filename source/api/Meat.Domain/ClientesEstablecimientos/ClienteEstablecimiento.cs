using Meat.Domain.Establecimientos;
using Meat.Domain.Clientes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.ClientesEstablecimientos
{
    public class ClienteEstablecimiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }
        public string CodigoRenspa { get; set; }
        public string NumeroCUIG { get; set; }
    }
}
