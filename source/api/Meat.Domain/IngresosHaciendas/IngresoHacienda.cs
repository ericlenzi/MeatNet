using Meat.Domain.Clientes;
using Meat.Domain.ClientesEstablecimientos;
using Meat.Domain.Establecimientos;
using Meat.Domain.OrigenesHaciendas;
using Meat.Domain.Provincias;
using Meat.Domain.TiposEstadosIngresos;
using Meat.Domain.Tropas;
using Meat.Domain.UsosHaciendas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHacienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public long NumeroIngreso { get; set; }            // correlativo por establecimiento

        // Destino (aporta el filtro por empresa via Establecimiento.Empresa)
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        public DateTime FechaHoraIngreso { get; set; }

        // DT-e (datos, no entidad)
        public string NumeroDte { get; set; }
        public DateTime FechaEmisionDte { get; set; }

        // Dueño / consignatario
        public Guid ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        // Procedencia (RENSPA / CUIG)
        public Guid ClienteEstablecimientoId { get; set; }
        public virtual ClienteEstablecimiento ClienteEstablecimiento { get; set; }

        // Origen geografico de la hacienda
        public int ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }
        public string Localidad { get; set; }              // texto por ahora

        // Clasificacion
        public string OrigenHaciendaId { get; set; }
        public virtual OrigenHacienda OrigenHacienda { get; set; }
        public string UsoHaciendaId { get; set; }
        public virtual UsoHacienda UsoHacienda { get; set; }

        // Transporte (texto libre - v1)
        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string PatenteCamion { get; set; }
        public string PatenteJaula { get; set; }

        // Pesaje del camion (jaula entera, kg)
        public double PesoBruto { get; set; }
        public double Tara { get; set; }
        public double PesoNeto { get; set; }               // = Bruto - Tara

        // Estado
        public string EstadoIngresoId { get; set; }
        public virtual TipoEstadoIngreso EstadoIngreso { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public Guid? UsuarioAprobacionId { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public virtual ICollection<IngresoHaciendaPesada> Pesadas { get; set; }
        public virtual ICollection<IngresoHaciendaUbicacion> Ubicaciones { get; set; }
        public virtual ICollection<Tropa> Tropas { get; set; }
    }
}
