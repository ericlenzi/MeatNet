namespace Meat.Application.Categorias.GetCategorias
{
    public class GetCategoriasItem
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoSexoId { get; set; }
        public string TipoSexoNombre { get; set; }
        public string CodigoMaterialDesde { get; set; }
        public string CodigoMaterialHasta { get; set; }
        public bool Activo { get; set; }
    }
}
