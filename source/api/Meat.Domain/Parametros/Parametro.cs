using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Parametros
{
    public class Parametro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public string Codigo { get; set; }
        public string Valor { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
