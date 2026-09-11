using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.AnalisisFaena.GetAnalisisFaena
{
    public class GetAnalisisFaenaRequest : RequestBase, IRequest<GetAnalisisFaenaResponse>
    {

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetAnalisisFaenaResponse
    {
        // Cabecera
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public DateTime Fecha { get; set; }
        public string EspecieId { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        public string EstablecimientoNombre { get; set; }
        /// <summary>Puesto (palco) donde se faena la jornada. Lo declara la lista de matanza.</summary>
        public string PuestoCodigo { get; set; }
        public string PuestoNombre { get; set; }

        // Resumen de la jornada
        public int AnimalesFaenados { get; set; }

        /// <summary>Piezas que son carne: las de reses condenadas no cuentan (R-A6).</summary>
        public int Piezas { get; set; }

        /// <summary>Kg de romaneo que llegaron a camara. No incluye las reses condenadas (R-A6).</summary>
        public double KgFaena { get; set; }

        /// <summary>Kg vivos de los animales faenados. Null si ninguna tropa tiene peso de ingreso (R-A3).</summary>
        public double? KgVivos { get; set; }

        /// <summary>Rinde caliente (%). Null cuando no hay peso vivo para calcularlo (R-A3).</summary>
        public double? RindeCaliente { get; set; }

        /// <summary>Animales faenados cuya tropa no tiene peso de ingreso: quedan fuera del rinde.</summary>
        public int AnimalesSinPesoVivo { get; set; }

        // Banda de rinde esperable de la especie (R-A7). Null si la especie no la tiene
        // configurada, y en ese caso no se avisa nada.
        public double? RindeMinimo { get; set; }
        public double? RindeMaximo { get; set; }

        /// <summary>El rinde quedó fuera de la banda de la especie: casi siempre es el peso vivo.</summary>
        public bool RindeFueraDeRango { get; set; }

        // --- Rinde frio ESTIMADO (R-A8) ---
        // No hay segunda pesada: el frio se proyecta con la merma de oreo configurada. Todo este
        // bloque viene en null cuando no hay coeficiente, y ahi la pantalla no muestra nada.

        /// <summary>Merma de oreo aplicada (%). Null si no hay coeficiente configurado.</summary>
        public double? MermaOreo { get; set; }

        /// <summary>De donde salio el coeficiente: ESTABLECIMIENTO o ESPECIE.</summary>
        public string MermaOreoOrigen { get; set; }

        /// <summary>Kg de faena que quedarian tras el oreo.</summary>
        public double? KgFaenaFrio { get; set; }

        /// <summary>Kg que se perderian en el oreo.</summary>
        public double? KgMermaOreo { get; set; }

        /// <summary>Rinde frio estimado (%). Null sin peso vivo o sin coeficiente.</summary>
        public double? RindeFrio { get; set; }

        // --- Produccion ESTIMADA de subproductos (R-A9) ---
        // Cuero, sebo y menudencias no se pesan: se estiman con el rendimiento configurado sobre
        // el peso de la res faenada. No son existencia; son produccion informada.

        /// <summary>
        /// Base de la estimacion: los kg que fueron a camara, sin lo condenado. La res que la
        /// inspeccion condeno se va entera al digestor, visceras incluidas, asi que estimar su
        /// menudencia seria informar produccion que no existe. El cuero de esa res en la practica
        /// se recupera, pero distinguirlo pide marcar subproducto por subproducto y eso recien
        /// vale la pena cuando se pesen de verdad (O-3 en EvaluacionFaena.md).
        /// </summary>
        public double KgBaseSubproductos { get; set; }

        public IEnumerable<SubproductoEstimadoItem> SubproductosEstimados { get; set; }
            = new List<SubproductoEstimadoItem>();

        /// <summary>Total de kg estimados de subproducto.</summary>
        public double KgSubproductosEstimados { get; set; }

        public int PiezasLiberadas { get; set; }

        // --- Merma sanitaria (R-A6) ---
        // El rinde no se retoca: los decomisos se informan aparte, para que la caida del rinde
        // quede explicada en vez de ser un hueco.

        /// <summary>Reses condenadas enteras (R-E23).</summary>
        public int AnimalesDecomisados { get; set; }

        /// <summary>Kg de las reses condenadas enteras.</summary>
        public double KgDecomisoTotal { get; set; }

        /// <summary>Medias reses condenadas por separado (R-E27).</summary>
        public int PiezasDecomisadas { get; set; }

        /// <summary>Kg de esas medias reses condenadas.</summary>
        public double KgDecomisoPieza { get; set; }

        /// <summary>Medias reses con recorte sanitario parcial (R-E24).</summary>
        public int PiezasConDecomisoParcial { get; set; }

        /// <summary>Kg retirados por recortes parciales.</summary>
        public double KgDecomisoParcial { get; set; }

        /// <summary>Total de kg condenados de la jornada.</summary>
        public double KgDecomisados { get; set; }

        /// <summary>Kg condenados sobre kg vivos (%). Null sin peso vivo, igual que el rinde.</summary>
        public double? MermaSanitaria { get; set; }

        public IEnumerable<AnalisisClienteItem> PorCliente { get; set; } = new List<AnalisisClienteItem>();
        public IEnumerable<PlanVsRealItem> PlanVsReal { get; set; } = new List<PlanVsRealItem>();
        public IEnumerable<TipificacionConsolidadaItem> Tipificaciones { get; set; } = new List<TipificacionConsolidadaItem>();
        public IEnumerable<DispersionPesoItem> Dispersion { get; set; } = new List<DispersionPesoItem>();
        public IEnumerable<DestinoCamaraItem> Camaras { get; set; } = new List<DestinoCamaraItem>();
        public IEnumerable<DecomisoMotivoItem> Decomisos { get; set; } = new List<DecomisoMotivoItem>();
    }

    /// <summary>Los decomisos de la jornada agrupados por su causa sanitaria.</summary>
    public class DecomisoMotivoItem
    {
        public string MotivoCodigo { get; set; }
        public string MotivoNombre { get; set; }

        /// <summary>Reses condenadas enteras por este motivo.</summary>
        public int Animales { get; set; }

        /// <summary>Medias reses condenadas enteras por este motivo.</summary>
        public int PiezasCondenadas { get; set; }

        /// <summary>Medias reses con recorte parcial por este motivo.</summary>
        public int Piezas { get; set; }

        /// <summary>Kg condenados por este motivo (la res entera, o los kilos del recorte).</summary>
        public double Kg { get; set; }
    }

    /// <summary>El corte de facturacion del servicio de faena.</summary>
    public class SubproductoEstimadoItem
    {
        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }

        /// <summary>Porcentaje del peso de la res con el que se estimo.</summary>
        public double Porcentaje { get; set; }

        public double Kg { get; set; }
    }

    public class AnalisisClienteItem
    {
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public int AnimalesFaenados { get; set; }
        public int Piezas { get; set; }
        public double KgFaena { get; set; }
        public double? KgVivos { get; set; }
        public double? RindeCaliente { get; set; }
        /// <summary>Participacion en los kg de faena de la jornada (%).</summary>
        public double ParticipacionKg { get; set; }
        /// <summary>Kg condenados de este cliente (res entera o recorte).</summary>
        public double KgDecomisados { get; set; }
    }

    public class PlanVsRealItem
    {
        public long NumeroTropa { get; set; }
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string CorralNombre { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Secuencia { get; set; }
        public int Planificado { get; set; }
        public int Faenado { get; set; }
        public int Diferencia { get; set; }
        /// <summary>Faenado sobre planificado (%).</summary>
        public double Cumplimiento { get; set; }
        public double? PesoPromedioVivo { get; set; }
    }

    public class TipificacionConsolidadaItem
    {
        public Guid? TipificacionId { get; set; }
        public string Descripcion { get; set; }
        public string MaterialNombre { get; set; }
        public int Piezas { get; set; }
        public double KgFaena { get; set; }
        public double PesoPromedio { get; set; }
        /// <summary>Participacion en los kg de la jornada (%).</summary>
        public double ParticipacionKg { get; set; }
    }

    public class DispersionPesoItem
    {
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Piezas { get; set; }
        public double PesoPromedio { get; set; }
        public double PesoMinimo { get; set; }
        public double PesoMaximo { get; set; }
        /// <summary>Piezas cuyo peso quedo fuera del rango de su tipificacion.</summary>
        public int PiezasFueraRango { get; set; }
    }

    public class DestinoCamaraItem
    {
        public string AlmacenNombre { get; set; }
        public string MaterialNombre { get; set; }
        public int Cantidad { get; set; }
        public double Peso { get; set; }
    }
}
