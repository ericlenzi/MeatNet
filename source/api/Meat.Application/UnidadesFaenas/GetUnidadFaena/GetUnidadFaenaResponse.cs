namespace Meat.Application.UnidadesFaenas.GetUnidadFaena
{
    public class GetUnidadFaenaResponse
    {
        public string Codigo { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string TipoMaterialId { get; set; }
        public string TipoMaterialNombre { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
