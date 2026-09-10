using Meat.Domain.Almacenes;
using Meat.Domain.MotivosDecomisos;
using Meat.Domain.TiposContusiones;
using Meat.Domain.Tipificaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Pieza fisica pesada de un Romaneo. PORCINO: 1 pieza (Letra null, RES).
    /// VACUNO: 2 piezas (MEDIA RES, Letra "A" / "B"). Peso es la cache desnormalizada
    /// de la medicion PESO (canonico para elegir la Tipificacion por rango y para KG).
    /// </summary>
    public class RomaneoPieza : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid RomaneoId { get; set; }
        public virtual Romaneo Romaneo { get; set; }

        public string Letra { get; set; }                      // "A" / "B"; null para porcino

        public Guid AlmacenDestinoId { get; set; }             // camara destino de esta pieza: default del renglon (LM), editable y obligatoria
        public virtual Almacen AlmacenDestino { get; set; }

        public Guid? TipificacionId { get; set; }
        public virtual Tipificacion Tipificacion { get; set; }

        // Contusion de esta media res. Es el unico de los cuatro datos del palco que se registra
        // por pieza: el golpe esta en una media res concreta, no en el animal entero.
        public string TipoContusionId { get; set; }
        public virtual TipoContusion TipoContusion { get; set; }

        public double Peso { get; set; }                       // cache de la medicion PESO

        public bool PesoFueraRango { get; set; }               // el peso quedo fuera del rango de la Tipificacion y el operario forzo el registro

        // Media res condenada entera (R-E27): la inspeccion condena ESTA pieza y la otra del
        // animal sigue su curso normal. Es el nivel intermedio entre la res condenada
        // (Romaneo.DecomisoTotal) y el recorte de kilos (PesoDecomisado). Como la res condenada,
        // se pesa pero no se tipifica ni entra a camara, y sus kilos van enteros a la merma.
        public bool Decomisada { get; set; }

        // Motivo del decomiso de esta media res, sea la condena entera (R-E27) o el recorte
        // parcial (R-E24): la causa es la misma cosa, lo que cambia es el alcance.
        //
        // Decomiso parcial: la inspeccion retira kilos y la pieza sigue su curso a camara. Motivo
        // y kilos van juntos: si hay uno hay el otro. PesoDecomisado NO ajusta Peso, que es lo que
        // dio la balanza y lo que entra a camara; los kilos retirados se informan aparte como
        // merma sanitaria (AnalisisFaena.md).
        //
        // En la media res condenada PesoDecomisado queda en cero y la merma la aporta el Peso
        // entero: guardar el mismo numero dos veces solo abre la puerta a que difieran.
        public string MotivoDecomisoId { get; set; }
        public virtual MotivoDecomiso MotivoDecomiso { get; set; }

        public double PesoDecomisado { get; set; }

        // Marca por pieza: la Liberacion la usa para saltear lo ya procesado (idempotencia,
        // R-L7) y para bloquear la edicion de la pieza (R-L3).
        public bool Liberado { get; set; }

        public virtual ICollection<RomaneoPiezaMedicion> Mediciones { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
