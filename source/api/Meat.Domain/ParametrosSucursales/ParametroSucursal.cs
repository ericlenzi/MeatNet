using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Parametros;

namespace Meat.Domain.ParametrosSucursales
{
    public class ParametroSucursal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid ParametroId { get; set; }
        public Guid SucursalId { get; set; }
        public string Valor { get; set; }
        public virtual Parametro Parametro { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
