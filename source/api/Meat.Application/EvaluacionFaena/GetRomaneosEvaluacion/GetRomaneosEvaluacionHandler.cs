using MediatR;
using Meat.Application.IngresosHaciendas; // FamiliaAlmacen
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EvaluacionFaena.GetRomaneosEvaluacion
{
    /// <summary>
    /// Romaneos de la jornada para revisarlos antes de liberar (Ciclo I paso 4): cada pieza con
    /// su peso, tipificacion, camara y el material que va a producir. Es la vista de trabajo de
    /// la Evaluacion de Faena y la fuente de la planilla impresa.
    ///
    /// A diferencia de GetRomaneosJornada (la grilla del Tipificador, paso 3), trae los Id de
    /// pieza para poder editarlas, el material resultante y el estado de liberacion.
    /// </summary>
    public class GetRomaneosEvaluacionHandler : IRequestHandler<GetRomaneosEvaluacionRequest, GetRomaneosEvaluacionResponse>
    {
        private readonly MeatContext context;

        public GetRomaneosEvaluacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRomaneosEvaluacionResponse> Handle(GetRomaneosEvaluacionRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            var data = await (
                from r in this.context.Romaneos
                join t in this.context.Tropas on r.TropaId equals t.Id
                join uf in this.context.UnidadesFaenas on r.UnidadFaenaId equals uf.Id
                join d in this.context.ListasMatanzasDetalles on r.ListaMatanzaDetalleId equals d.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Codigo
                where r.ListaMatanzaId == lm.Id
                orderby r.NumeroRomaneo
                select new RomaneoEvaluacionItem
                {
                    Id = r.Id,
                    NumeroRomaneo = r.NumeroRomaneo,
                    NumeroGarron = r.NumeroGarron,
                    TropaId = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    TipoEspecieId = te.Codigo,
                    TipoEspecieNombre = te.Nombre,
                    UnidadFaenaNombre = uf.Nombre,
                    Fecha = r.Fecha,
                    Anulado = r.Anulado,
                    Liberado = r.Liberado,
                    FechaLiberacion = r.FechaLiberacion,
                    DecomisoTotal = r.DecomisoTotal,
                    MotivoDecomisoNombre = r.MotivoDecomiso != null ? r.MotivoDecomiso.Nombre : null,
                    PesoTotal = r.Piezas.Sum(p => p.Peso),
                    Piezas = r.Piezas
                        .OrderBy(p => p.Letra)
                        .Select(p => new PiezaEvaluacionItem
                        {
                            Id = p.Id,
                            Letra = p.Letra,
                            Peso = p.Peso,
                            PesoFueraRango = p.PesoFueraRango,
                            Liberado = p.Liberado,
                            AlmacenDestinoId = p.AlmacenDestinoId,
                            AlmacenDestinoNombre = p.AlmacenDestino != null ? p.AlmacenDestino.Nombre : null,
                            TipificacionId = p.TipificacionId,
                            TipificacionDescripcion = p.Tipificacion != null ? p.Tipificacion.Descripcion : null,
                            MaterialId = p.Tipificacion != null ? p.Tipificacion.MaterialId : null,
                            MaterialCodigo = p.Tipificacion != null && p.Tipificacion.Material != null ? p.Tipificacion.Material.CodigoMaterial : null,
                            MaterialNombre = p.Tipificacion != null && p.Tipificacion.Material != null ? p.Tipificacion.Material.Nombre : null,
                            MotivoDecomisoNombre = p.MotivoDecomiso != null ? p.MotivoDecomiso.Nombre : null,
                            PesoDecomisado = p.PesoDecomisado
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            var camaras = await (
                from a in this.context.Almacenes
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where a.EstablecimientoId == lm.EstablecimientoId
                    && ta.Familia == FamiliaAlmacen.Camara
                    && a.Activo
                orderby a.Nombre
                select new CamaraOpcion { Id = a.Id, Nombre = a.Nombre })
                .ToListAsync(cancellationToken);

            // Los totales miran solo lo no anulado: es la carne que existe.
            var vigentes = data.Where(r => !r.Anulado).ToList();

            return new GetRomaneosEvaluacionResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                Fecha = lm.Fecha,
                EspecieId = lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                EstablecimientoNombre = lm.Establecimiento.Nombre,
                TotalRomaneos = vigentes.Count,
                TotalPiezas = vigentes.Sum(r => r.Piezas.Count()),
                TotalKg = vigentes.Sum(r => r.PesoTotal),
                PiezasLiberadas = vigentes.Sum(r => r.Piezas.Count(p => p.Liberado)),
                // Merma sanitaria de la jornada: la res condenada aporta todos sus kilos, la
                // media res con recorte parcial solo los que retiro la inspeccion.
                TotalDecomisosTotales = vigentes.Count(r => r.DecomisoTotal),
                TotalKgDecomisados = vigentes.Sum(r => r.DecomisoTotal
                    ? r.PesoTotal
                    : r.Piezas.Sum(p => p.PesoDecomisado)),
                Data = data,
                Camaras = camaras
            };
        }
    }
}
