using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.AnalisisFaena.GetAnalisisFaena
{
    /// <summary>
    /// Analisis de la jornada (Ciclo I paso 4b): rinde caliente, plan vs. real, tipificacion
    /// consolidada, dispersion de pesos y destino a camaras, todo abierto por cliente. Solo
    /// lectura (R-A1). Ver docs/manuales/AnalisisFaena.md.
    ///
    /// El rinde es CALIENTE y contra el peso VIVO DE INGRESO: no hay pesada en playa (desbaste)
    /// ni segunda pesada tras el oreo, asi que sale subestimado y no descuenta decomisos. La
    /// pantalla muestra esos supuestos junto al numero.
    /// </summary>
    public class GetAnalisisFaenaHandler : IRequestHandler<GetAnalisisFaenaRequest, GetAnalisisFaenaResponse>
    {
        private readonly MeatContext context;

        public GetAnalisisFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetAnalisisFaenaResponse> Handle(GetAnalisisFaenaRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId
                    && x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            // R-A2: los romaneos anulados no son carne y quedan fuera de todo.
            var piezas = await (
                from p in this.context.RomaneosPiezas
                join r in this.context.Romaneos on p.RomaneoId equals r.Id
                join d in this.context.ListasMatanzasDetalles on r.ListaMatanzaDetalleId equals d.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Id
                join t in this.context.Tropas on r.TropaId equals t.Id
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                where r.ListaMatanzaId == lm.Id && !r.Anulado
                select new
                {
                    PiezaId = p.Id,
                    RomaneoId = r.Id,
                    p.Peso,
                    p.PesoFueraRango,
                    p.Liberado,
                    p.TipificacionId,
                    TipificacionDescripcion = p.Tipificacion != null ? p.Tipificacion.Descripcion : null,
                    MaterialNombre = p.Tipificacion != null && p.Tipificacion.Material != null ? p.Tipificacion.Material.Nombre : null,
                    TipoEspecieId = te.Id,
                    TipoEspecieNombre = te.Nombre,
                    ClienteId = c.Id,
                    ClienteNombre = c.Nombre,
                    TropaId = t.Id
                }).ToListAsync(cancellationToken);

            // Renglones con su peso vivo promedio: la ubicacion se identifica por la misma clave
            // que el renglon (Tropa, Corral, TipoEspecie).
            var renglones = await (
                from d in this.context.ListasMatanzasDetalles
                join t in this.context.Tropas on d.TropaId equals t.Id
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join a in this.context.Almacenes on d.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Id
                where d.ListaMatanzaId == lm.Id
                select new
                {
                    d.Id,
                    d.TropaId,
                    t.NumeroTropa,
                    ClienteId = c.Id,
                    ClienteNombre = c.Nombre,
                    CorralNombre = a.Nombre,
                    TipoEspecieNombre = te.Nombre,
                    d.Secuencia,
                    d.Cantidad,
                    d.CantidadFaenada,
                    // Peso promedio del animal vivo al ingresar. Null si la tropa no tiene peso
                    // cargado: en ese caso el rinde de ese renglon no se calcula (R-A3).
                    PesoPromedioVivo = (
                        from u in this.context.IngresosHaciendasUbicaciones
                        where u.TropaId == d.TropaId && u.AlmacenId == d.AlmacenId && u.TipoEspecieId == d.TipoEspecieId
                        select (double?)u.PesoPromedio).FirstOrDefault()
                }).ToListAsync(cancellationToken);

            var kgFaena = piezas.Sum(p => p.Peso);
            var animalesFaenados = piezas.Select(p => p.RomaneoId).Distinct().Count();

            // Kg vivos: se prorratea el promedio de la ubicacion por los animales faenados. Los
            // renglones sin peso de ingreso no suman y se informan aparte (R-A3).
            var renglonesConPeso = renglones.Where(r => r.PesoPromedioVivo.HasValue && r.PesoPromedioVivo.Value > 0).ToList();
            var kgVivos = renglonesConPeso.Sum(r => r.CantidadFaenada * r.PesoPromedioVivo.Value);
            var animalesSinPesoVivo = renglones
                .Where(r => !r.PesoPromedioVivo.HasValue || r.PesoPromedioVivo.Value <= 0)
                .Sum(r => r.CantidadFaenada);

            var response = new GetAnalisisFaenaResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                Fecha = lm.Fecha,
                EspecieId = lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                EstablecimientoNombre = lm.Establecimiento.Nombre,
                AnimalesFaenados = animalesFaenados,
                Piezas = piezas.Count,
                KgFaena = kgFaena,
                KgVivos = kgVivos > 0 ? kgVivos : (double?)null,
                RindeCaliente = kgVivos > 0 ? Math.Round(kgFaena / kgVivos * 100, 2) : (double?)null,
                AnimalesSinPesoVivo = animalesSinPesoVivo,
                PiezasLiberadas = piezas.Count(p => p.Liberado)
            };

            // --- Por cliente: el corte por el que se discute el resultado ---
            var kgVivosPorCliente = renglonesConPeso
                .GroupBy(r => r.ClienteId)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.CantidadFaenada * r.PesoPromedioVivo.Value));

            response.PorCliente = piezas
                .GroupBy(p => new { p.ClienteId, p.ClienteNombre })
                .Select(g =>
                {
                    var kg = g.Sum(x => x.Peso);
                    kgVivosPorCliente.TryGetValue(g.Key.ClienteId, out var vivos);
                    return new AnalisisClienteItem
                    {
                        ClienteId = g.Key.ClienteId,
                        ClienteNombre = g.Key.ClienteNombre,
                        AnimalesFaenados = g.Select(x => x.RomaneoId).Distinct().Count(),
                        Piezas = g.Count(),
                        KgFaena = kg,
                        KgVivos = vivos > 0 ? vivos : (double?)null,
                        RindeCaliente = vivos > 0 ? Math.Round(kg / vivos * 100, 2) : (double?)null,
                        ParticipacionKg = kgFaena > 0 ? Math.Round(kg / kgFaena * 100, 2) : 0
                    };
                })
                .OrderByDescending(x => x.KgFaena)
                .ToList();

            // --- Plan vs. real ---
            response.PlanVsReal = renglones
                .OrderBy(r => r.Secuencia)
                .Select(r => new PlanVsRealItem
                {
                    NumeroTropa = r.NumeroTropa,
                    ClienteId = r.ClienteId,
                    ClienteNombre = r.ClienteNombre,
                    CorralNombre = r.CorralNombre,
                    TipoEspecieNombre = r.TipoEspecieNombre,
                    Secuencia = r.Secuencia,
                    Planificado = r.Cantidad,
                    Faenado = r.CantidadFaenada,
                    Diferencia = r.CantidadFaenada - r.Cantidad,
                    Cumplimiento = r.Cantidad > 0 ? Math.Round((double)r.CantidadFaenada / r.Cantidad * 100, 2) : 0,
                    PesoPromedioVivo = r.PesoPromedioVivo
                })
                .ToList();

            // --- Tipificacion consolidada ---
            response.Tipificaciones = piezas
                .GroupBy(p => new { p.TipificacionId, p.TipificacionDescripcion, p.MaterialNombre })
                .Select(g =>
                {
                    var kg = g.Sum(x => x.Peso);
                    return new TipificacionConsolidadaItem
                    {
                        TipificacionId = g.Key.TipificacionId,
                        Descripcion = g.Key.TipificacionDescripcion ?? g.Key.TipificacionId,
                        MaterialNombre = g.Key.MaterialNombre,
                        Piezas = g.Count(),
                        KgFaena = kg,
                        PesoPromedio = Math.Round(kg / g.Count(), 2),
                        ParticipacionKg = kgFaena > 0 ? Math.Round(kg / kgFaena * 100, 2) : 0
                    };
                })
                .OrderByDescending(x => x.KgFaena)
                .ToList();

            // --- Pesos y dispersion ---
            response.Dispersion = piezas
                .GroupBy(p => new { p.TipoEspecieId, p.TipoEspecieNombre })
                .Select(g => new DispersionPesoItem
                {
                    TipoEspecieId = g.Key.TipoEspecieId,
                    TipoEspecieNombre = g.Key.TipoEspecieNombre,
                    Piezas = g.Count(),
                    PesoPromedio = Math.Round(g.Average(x => x.Peso), 2),
                    PesoMinimo = g.Min(x => x.Peso),
                    PesoMaximo = g.Max(x => x.Peso),
                    PiezasFueraRango = g.Count(x => x.PesoFueraRango)
                })
                .OrderBy(x => x.TipoEspecieNombre)
                .ToList();

            // --- Destino a camaras: lo que dejo la Liberacion. Vacio si todavia no se libero. ---
            var piezasIds = piezas.Select(p => p.PiezaId).ToList();
            var camaras = await (
                from mc in this.context.MovimientosCamaras
                join a in this.context.Almacenes on mc.AlmacenId equals a.Id
                join m in this.context.Materiales on mc.MaterialId equals m.Id
                where mc.RomaneoPiezaOrigenId != null && piezasIds.Contains(mc.RomaneoPiezaOrigenId.Value)
                group new { mc.Cantidad, mc.Peso } by new { AlmacenNombre = a.Nombre, MaterialNombre = m.Nombre } into g
                select new DestinoCamaraItem
                {
                    AlmacenNombre = g.Key.AlmacenNombre,
                    MaterialNombre = g.Key.MaterialNombre,
                    Cantidad = g.Sum(x => x.Cantidad),
                    Peso = g.Sum(x => x.Peso)
                }).ToListAsync(cancellationToken);

            // El material que se cuarteo por completo queda en cero: no es destino, es transito.
            response.Camaras = camaras
                .Where(c => c.Cantidad != 0)
                .OrderBy(c => c.AlmacenNombre).ThenBy(c => c.MaterialNombre)
                .ToList();

            return response;
        }
    }
}
