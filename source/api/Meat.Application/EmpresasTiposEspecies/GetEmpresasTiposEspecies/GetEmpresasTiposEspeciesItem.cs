using System;

namespace Meat.Application.EmpresasTiposEspecies.GetEmpresasTiposEspecies
{
    public class GetEmpresasTiposEspeciesItem
    {
        public Guid Id { get; set; }

        /// <summary>Codigo de la categoria en el catalogo global. Es la FK que guarda la operacion.</summary>
        public string TipoEspecieId { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoSexoId { get; set; }
        public string TipoSexoNombre { get; set; }

        /// <summary>Peso teorico con el que trabaja la empresa. Es el que usan los calculos.</summary>
        public double PesoTeorico { get; set; }

        /// <summary>Peso sugerido del catalogo. Se muestra al lado para que se vea el desvio.</summary>
        public double PesoTeoricoReferencia { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }

        /// <summary>Estado en el catalogo global. Si esta en false, la categoria no se puede usar
        /// aunque la empresa la tenga activa.</summary>
        public bool TipoEspecieActivo { get; set; }
    }
}
