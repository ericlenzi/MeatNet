using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.UnidadesFaenas.UpdateUnidadFaena
{
    public class UpdateUnidadFaenaRequestFromBody
    {
        [Required]
        public string EspecieId { get; set; }
        [Required]
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string TipoMaterialId { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
