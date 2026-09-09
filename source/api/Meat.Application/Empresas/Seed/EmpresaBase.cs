using System.Collections.Generic;

namespace Meat.Application.Empresas.Seed
{
    /// <summary>Forma del archivo empresa-base.json.</summary>
    public class EmpresaBase
    {
        public List<ParametroBase> Parametros { get; set; } = new List<ParametroBase>();
        public List<DestinoBase> DestinosComerciales { get; set; } = new List<DestinoBase>();
        public List<TipoEspecieBase> TiposEspecies { get; set; } = new List<TipoEspecieBase>();
        public List<UnidadFaenaBase> UnidadesFaenas { get; set; } = new List<UnidadFaenaBase>();
    }

    public class ParametroBase
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }

    public class DestinoBase
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Favorito { get; set; }
    }

    public class TipoEspecieBase
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public double PesoTeorico { get; set; }
    }

    public class UnidadFaenaBase
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string TipoMaterialId { get; set; }
    }
}
