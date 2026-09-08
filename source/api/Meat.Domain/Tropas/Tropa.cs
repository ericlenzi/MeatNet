using Meat.Domain.Especies;
using Meat.Domain.IngresosHaciendas;
using Meat.Domain.TiposEstadosTropas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Tropas
{
    public class Tropa : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        // Contexto de numeracion (NumeradorTropa: ClienteEstablecimiento + Especie)
        public Guid ClienteEstablecimientoId { get; set; }
        public string EspecieCodigo { get; set; }
        public virtual Especie Especie { get; set; }

        public long NumeroTropa { get; set; }              // correlativo, no reutilizable

        public string EstadoTropaId { get; set; }
        public virtual TipoEstadoTropa EstadoTropa { get; set; }

        public DateTime FechaRecepcion { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
