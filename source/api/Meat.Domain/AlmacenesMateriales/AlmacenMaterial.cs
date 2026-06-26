using Meat.Domain.Almacenes;
using Meat.Domain.Enums;
using Meat.Domain.Materiales;
using Meat.Domain.TiposAlmacenes;
using Meat.Domain.TiposAnimales;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.AlmacenesMateriales
{
    public class AlmacenMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public Guid AlmacenId { get; set; }
        public virtual Almacen Almacen { get; set; }
        public Guid MaterialId { get; set; }
        public Material Material { get; set; }
        public string TipoAnimalId { get; set; }
        public TipoAnimal TipoAnimal { get; set; }
        public int Cantidad { get; set; }
    }
}