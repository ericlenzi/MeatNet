using Meat.Domain.Especies;
using Meat.Domain.Establecimientos;
using Meat.Domain.ListasMatanzas;
using Meat.Domain.Tropas;
using Meat.Domain.Conformaciones;
using Meat.Domain.Denticiones;
using Meat.Domain.GradosEngrasamiento;
using Meat.Domain.MotivosDecomisos;
using Meat.Domain.Puestos;
using Meat.Domain.Tipificadores;
using Meat.Domain.TiposMediciones;
using Meat.Domain.UnidadesFaenas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Meat.Domain.Empresas;
using Meat.Domain.Shared;

namespace Meat.Domain.Romaneos
{
    /// <summary>
    /// Romaneo de un animal faenado (Ciclo I paso 3). Es la unidad que consume stock:
    /// 1 romaneo = 1 animal = CantidadFaenada += 1 en el renglon de la LM, sin importar
    /// cuantas piezas tenga (VACUNO = 2 medias reses A/B; PORCINO = 1 res). Cuelga de la
    /// LM EN_EJECUCION (jornada) y del renglon elegido.
    /// </summary>
    public class Romaneo : ITenantScoped
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid ListaMatanzaId { get; set; }               // jornada (LM en ejecucion)
        public virtual ListaMatanza ListaMatanza { get; set; }

        public Guid EstablecimientoId { get; set; }            // denormalizado de la LM: es el alcance del correlativo NumeroRomaneo
        public virtual Establecimiento Establecimiento { get; set; }

        public Guid ListaMatanzaDetalleId { get; set; }        // renglon elegido (Tropa+Corral+TipoEspecie)
        public virtual ListaMatanzaDetalle ListaMatanzaDetalle { get; set; }

        public Guid TropaId { get; set; }                      // denormalizado del renglon (trazabilidad)
        public virtual Tropa Tropa { get; set; }

        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        // Cabecera del puesto: donde se tipifico, quien tipifico y con que metodo se midio.
        // Los tres se proponen solos (el puesto lo trae la LM, el tipificador y el metodo salen
        // de la configuracion) y quedan en el romaneo porque son un hecho de la jornada: si
        // manana cambia la configuracion, lo que ya se faeno se sigue leyendo igual.
        // Nullables: los romaneos anteriores a la configuracion de puestos no los traen.
        public Guid? PuestoId { get; set; }
        public virtual Puesto Puesto { get; set; }

        public Guid? TipificadorId { get; set; }
        public virtual Tipificador Tipificador { get; set; }

        public string TipoMedicionId { get; set; }
        public virtual TipoMedicion TipoMedicion { get; set; }

        public Guid UnidadFaenaId { get; set; }              // RES / MEDIA RES; define nro de piezas

        // Ejes de la tipificacion oficial que se determinan mirando la res, no el animal en pie:
        // varian de una res a otra, asi que se capturan por romaneo y no en el master data. La
        // categoria (el tercer eje) viene de la Tipificacion de cada pieza.
        // Nullables: no toda especie tiene tipificacion oficial y los romaneos previos no la traen.
        public string ConformacionId { get; set; }
        public virtual Conformacion Conformacion { get; set; }

        public string GradoEngrasamientoId { get; set; }
        public virtual GradoEngrasamiento GradoEngrasamiento { get; set; }

        // Denticion: se mira la boca del animal, asi que es del romaneo y no de la pieza, igual
        // que los otros dos ejes que se determinan en el palco.
        public string DenticionId { get; set; }
        public virtual Denticion Denticion { get; set; }

        public virtual UnidadFaena UnidadFaena { get; set; }

        // Decomiso total (R-E23): la inspeccion condena la res entera. El animal se faeno igual
        // (consume el renglon de la LM y gasta numero de romaneo) y sus piezas se pesan, porque
        // esos kilos son la merma sanitaria de la jornada; pero no se tipifican ni generan
        // existencia de camara al liberar. El motivo es obligatorio cuando la marca esta puesta.
        public bool DecomisoTotal { get; set; }
        public string MotivoDecomisoId { get; set; }
        public virtual MotivoDecomiso MotivoDecomiso { get; set; }

        public int NumeroGarron { get; set; }                  // nro fisico de gancho; unico por LM
        public long NumeroRomaneo { get; set; }                // correlativo Numerador ROMANEO (Estab+Especie)

        public DateTime Fecha { get; set; }
        public Guid? UsuarioId { get; set; }

        public bool Anulado { get; set; }                      // correccion de errores (devuelve stock)

        // Liberacion (Ciclo I paso 4): al liberar la jornada el romaneo queda definitivo
        // (no admite edicion ni anulacion) y sus piezas ya generaron existencia de camara.
        public bool Liberado { get; set; }
        public DateTime? FechaLiberacion { get; set; }
        public Guid? UsuarioLiberacionId { get; set; }

        public virtual ICollection<RomaneoPieza> Piezas { get; set; }

        /// <summary>Empresa (tenant) duena del registro. La asigna el MeatContext en el alta.</summary>
        public string EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
