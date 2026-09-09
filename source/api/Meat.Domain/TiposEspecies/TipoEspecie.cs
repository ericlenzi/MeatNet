using Meat.Domain.Especies;
using Meat.Domain.TiposSexos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposEspecies
{
    /// <summary>
    /// Categoria de hacienda en pie (NOVILLO, VAQUILLONA, VACA, TORO, CAPON...).
    ///
    /// Es un catalogo comun a todas las empresas: son los codigos estandar del rubro, la misma
    /// familia que TipificacionOficial o Denticion, y lo mantiene el SUPERADMIN desde la empresa
    /// administrativa. Lo que cada empresa ajusta (peso teorico propio, codigo de ERP, si opera
    /// o no con la categoria) vive en EmpresaTipoEspecie.
    /// </summary>
    public class TipoEspecie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public string TipoSexoId { get; set; }
        public virtual TipoSexo TipoSexo { get; set; }

        /// <summary>
        /// Peso teorico de referencia del rubro, en kg. No lo usan los calculos: es el valor que
        /// se propone cuando una empresa da de alta la categoria en su configuracion, y desde
        /// ahi cada empresa lo ajusta por su cuenta.
        /// </summary>
        public double PesoTeoricoReferencia { get; set; }
        public bool Activo { get; set; }
    }
}
