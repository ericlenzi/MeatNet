namespace Meat.Application.TiposEspecies.GetTiposEspecies
{
    public class GetTiposEspeciesItem
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoSexoId { get; set; }
        public string TipoSexoNombre { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public double PesoTeorico { get; set; }
        public bool Activo { get; set; }
    }
}
