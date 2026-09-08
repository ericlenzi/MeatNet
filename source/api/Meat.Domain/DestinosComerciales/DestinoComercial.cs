using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.DestinosComerciales
{
    /// <summary>
    /// Catalogo: destinos comerciales de la media res / cuarto.
    /// </summary>
    public class DestinoComercial : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>Codigo de negocio, unico dentro de la empresa.</summary>
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }

        /// <summary>Destino favorito: a lo sumo uno en true (indice unico filtrado). Es el
        /// que el Tipificador propone Por Defecto.</summary>
        public bool Favorito { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
