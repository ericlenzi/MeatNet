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

            // Piezas por animal con las que se proyecta lo pendiente: la unidad de faena que el
            // Tipificador propone para la especie (R-E12), porque el renglon no declara unidad.
            var piezasPorAnimal = await this.context.UnidadesFaenas
                .Where(u => u.Activo && u.EspecieId == lm.EspecieId)
                .OrderByDescending(u => u.PorDefecto)
                .Select(u => (int?)u.PiezasPorAnimal)
                .FirstOrDefaultAsync(cancellationToken) ?? 1;

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

            // --- Ocupacion de camaras (R-E29) ---------------------------------------------------
            // Tres cosas distintas que se suman: lo que ya esta colgado de esta jornada, lo que
            // va a llegar segun el plan, y lo que la camara ya tenia de jornadas anteriores.
            var colgadas = await (
                from p in this.context.RomaneosPiezas
                join r in this.context.Romaneos on p.RomaneoId equals r.Id
                where r.ListaMatanzaId == lm.Id && !r.Anulado
                    // Lo condenado se peso pero no va a camara (R-E23, R-E27): no ocupa gancho.
                    && !r.DecomisoTotal && !p.Decomisada
                group p by p.AlmacenDestinoId into g
                select new { AlmacenId = g.Key, Piezas = g.Count(), Kg = g.Sum(x => x.Peso) })
                .ToListAsync(cancellationToken);

            // Pendiente por camara: los animales que faltan de cada renglon, por las piezas que
            // deja cada animal. El renglon sin camara declarada se informa aparte, para que los
            // numeros cierren en vez de desaparecer.
            var pendientes = await (
                from d in this.context.ListasMatanzasDetalles
                where d.ListaMatanzaId == lm.Id && d.Cantidad > d.CantidadFaenada
                group d by d.AlmacenDestinoId into g
                select new { AlmacenId = g.Key, Animales = g.Sum(x => x.Cantidad - x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            var camarasDeLaJornada = colgadas.Select(c => (Guid?)c.AlmacenId)
                .Concat(pendientes.Select(p => p.AlmacenId))
                .Distinct()
                .ToList();

            var idsCamaras = camarasDeLaJornada.Where(id => id.HasValue).Select(id => id.Value).ToList();

            var datosCamaras = await this.context.Almacenes
                .Where(a => idsCamaras.Contains(a.Id))
                .Select(a => new { a.Id, a.Nombre, a.Capacidad })
                .ToListAsync(cancellationToken);

            // Saldo previo: el log de camara sin los movimientos que genero ESTA jornada, que ya
            // estan contados en lo colgado. Si la jornada todavia no se libero no hay ninguno,
            // pero el Monitor tambien se abre sobre una jornada ya liberada.
            var saldoPrevio = await (
                from m in this.context.MovimientosCamaras
                join p in this.context.RomaneosPiezas on m.RomaneoPiezaOrigenId equals p.Id into pj
                from p in pj.DefaultIfEmpty()
                join r in this.context.Romaneos on p.RomaneoId equals r.Id into rj
                from r in rj.DefaultIfEmpty()
                where idsCamaras.Contains(m.AlmacenId)
                    && (r == null || r.ListaMatanzaId != lm.Id)
                group m by m.AlmacenId into g
                select new { AlmacenId = g.Key, Piezas = g.Sum(x => x.Cantidad), Kg = g.Sum(x => x.Peso) })
                .ToListAsync(cancellationToken);

            var ocupacion = camarasDeLaJornada
                .Select(id =>
                {
                    var datos = id.HasValue ? datosCamaras.FirstOrDefault(a => a.Id == id.Value) : null;
                    var colgada = colgadas.FirstOrDefault(c => c.AlmacenId == id);
                    var pendiente = pendientes.FirstOrDefault(p => p.AlmacenId == id);
                    var previo = id.HasValue ? saldoPrevio.FirstOrDefault(s => s.AlmacenId == id.Value) : null;

                    var piezasPendientes = (pendiente?.Animales ?? 0) * piezasPorAnimal;
                    var piezasPrevias = previo?.Piezas ?? 0;
                    var proyectadas = (colgada?.Piezas ?? 0) + piezasPendientes + piezasPrevias;
                    var capacidad = datos?.Capacidad ?? 0;

                    return new OcupacionCamaraItem
                    {
                        AlmacenId = id,
                        AlmacenNombre = datos?.Nombre ?? "Sin camara asignada",
                        PiezasColgadas = colgada?.Piezas ?? 0,
                        KgColgados = colgada?.Kg ?? 0,
                        PiezasPendientes = piezasPendientes,
                        PiezasSaldoPrevio = piezasPrevias,
                        KgSaldoPrevio = previo?.Kg ?? 0,
                        PiezasProyectadas = proyectadas,
                        Capacidad = capacidad,
                        PorcentajeOcupacion = capacidad > 0
                            ? Math.Round((double)proyectadas / capacidad * 100, 1)
                            : (double?)null,
                        Excedida = capacidad > 0 && proyectadas > capacidad
                    };
                })
                // Primero la que esta mas comprometida; la fila sin camara, al final.
                .OrderByDescending(o => o.AlmacenId.HasValue)
                .ThenByDescending(o => o.PorcentajeOcupacion ?? -1)
                .ThenBy(o => o.AlmacenNombre)
                .ToList();

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
                Fecha = lm.Fecha,
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
                PorRenglon = porRenglon,
                OcupacionCamaras = ocupacion
            };
        }
    }
}
