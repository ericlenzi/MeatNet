namespace Meat.Application.TiposEspecies.GetTiposEspecies
{
    public class GetTiposEspeciesItem
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoSexoId { get; set; }
        public string TipoSexoNombre { get; set; }
        public double PesoTeoricoReferencia { get; set; }
        public bool Activo { get; set; }
    }
}
