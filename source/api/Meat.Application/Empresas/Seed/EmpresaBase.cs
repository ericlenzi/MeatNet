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

    /// <summary>
    /// Categoria con la que arranca una empresa nueva. Solo se usa el Codigo: los datos de la
    /// categoria (nombre, especie, sexo, peso de referencia) viven en el catalogo global
    /// TiposEspecies, que administra la empresa ADM. El Nombre queda como etiqueta para que el
    /// archivo se lea.
    /// </summary>
    public class TipoEspecieBase
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
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
