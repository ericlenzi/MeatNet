using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.GetMonitorFaena
{
    /// <summary>Totales en vivo de la jornada para el Monitor de Faena (read-only).</summary>
    public class GetMonitorFaenaHandler : IRequestHandler<GetMonitorFaenaRequest, GetMonitorFaenaResponse>
    {
        private readonly MeatContext context;

        public GetMonitorFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMonitorFaenaResponse> Handle(GetMonitorFaenaRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .Include(x => x.Especie)
                .Include(x => x.Puesto)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            var porRenglon = await (
                from d in this.context.ListasMatanzasDetalles
                join t in this.context.Tropas on d.TropaId equals t.Id
                join a in this.context.Almacenes on d.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Codigo
                where d.ListaMatanzaId == lm.Id
                orderby d.Secuencia
                select new RenglonMonitorItem
                {
                    ListaMatanzaDetalleId = d.Id,
                    Secuencia = d.Secuencia,
                    NumeroTropa = t.NumeroTropa,
                    AlmacenNombre = a.Nombre,
                    AlmacenDestinoNombre = d.AlmacenDestino != null ? d.AlmacenDestino.Nombre : null,
                    TipoEspecieNombre = te.Nombre,
                    Cantidad = d.Cantidad,
                    CantidadFaenada = d.CantidadFaenada,
                    Pendiente = d.Cantidad - d.CantidadFaenada
                })
                .ToListAsync(cancellationToken);

            // Romaneos no anulados de la jornada: conteo, KG y ventana temporal (para el ritmo).
            var romaneos = await this.context.Romaneos
                .Where(r => r.ListaMatanzaId == lm.Id && !r.Anulado)
                .Select(r => new
                {
                    r.Fecha,
                    r.ListaMatanzaDetalleId,
                    r.NumeroRomaneo,
                    r.DecomisoTotal,
                    Peso = r.Piezas.Sum(p => p.Peso),
                    PesoCondenado = r.DecomisoTotal
                        ? r.Piezas.Sum(p => p.Peso)
                        : r.Piezas.Where(p => p.Decomisada).Sum(p => p.Peso),
                    PiezasCondenadas = r.DecomisoTotal ? 0 : r.Piezas.Count(p => p.Decomisada)
                })
                .ToListAsync(cancellationToken);

            var animales = romaneos.Count;

            // Los kilos del monitor son los de carne: lo condenado se faeno pero no va a camara,
            // asi que se cuenta aparte y no infla el total (R-E23, R-E27).
            var kg = romaneos.Sum(r => r.Peso - r.PesoCondenado);
            var decomisados = romaneos.Count(r => r.DecomisoTotal);
            var piezasCondenadas = romaneos.Sum(r => r.PiezasCondenadas);
            var kgDecomisados = romaneos.Sum(r => r.PesoCondenado);

            // Rango de numeros de romaneo ya registrados en cada renglon.
            var rangos = romaneos
                .GroupBy(r => r.ListaMatanzaDetalleId)
                .ToDictionary(g => g.Key, g => new { Desde = g.Min(x => x.NumeroRomaneo), Hasta = g.Max(x => x.NumeroRomaneo) });

            foreach (var renglon in porRenglon)
            {
                if (rangos.TryGetValue(renglon.ListaMatanzaDetalleId, out var rango))
                {
                    renglon.RomaneoDesde = rango.Desde;
                    renglon.RomaneoHasta = rango.Hasta;
                }
            }

            double ritmo = 0;
            if (animales > 1)
            {
                var horas = (DateTime.Now - romaneos.Min(r => r.Fecha)).TotalHours;
                if (horas > 0)
                    ritmo = animales / horas;
            }

            return new GetMonitorFaenaResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                EspecieNombre = lm.Especie != null ? lm.Especie.Nombre : lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                PuestoCodigo = lm.Puesto != null ? lm.Puesto.CodigoPuesto : null,
                PuestoNombre = lm.Puesto != null ? lm.Puesto.Nombre : null,
                TotalPlanificado = porRenglon.Sum(r => r.Cantidad),
                TotalFaenado = porRenglon.Sum(r => r.CantidadFaenada),
                TotalPendiente = porRenglon.Sum(r => r.Pendiente),
                AnimalesRomaneados = animales,
                KgTotales = kg,
                AnimalesDecomisados = decomisados,
                PiezasDecomisadas = piezasCondenadas,
                KgDecomisados = kgDecomisados,
                RitmoPorHora = Math.Round(ritmo, 1),
                PorRenglon = porRenglon
            };
        }
    }
}
