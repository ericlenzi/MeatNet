using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ExistenciaCamara.GetExistenciaCamara
{
    /// <summary>
    /// Saldo de existencia de camara, derivado del log MovimientosCamaras sumando Cantidad y
    /// Peso. Mismo patron que el En Pie derivado: el log es la fuente de verdad y el saldo se
    /// calcula, no se guarda. Ver docs/manuales/EvaluacionFaena.md.
    ///
    /// Se puede cortar por material (inventario), por proveedor (que tiene cada cliente) o por
    /// camara (de quien es lo que hay en cada una). El dueno de la hacienda se alcanza desde el
    /// movimiento por su TropaId denormalizado.
    ///
    /// Es la existencia que el Ciclo II (Despostada) va a consumir.
    /// </summary>
    public class GetExistenciaCamaraHandler : IRequestHandler<GetExistenciaCamaraRequest, GetExistenciaCamaraResponse>
    {
        private readonly MeatContext context;

        public GetExistenciaCamaraHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetExistenciaCamaraResponse> Handle(GetExistenciaCamaraRequest request, CancellationToken cancellationToken)
        {
            var agrupacion = string.IsNullOrWhiteSpace(request.AgruparPor)
                ? AgrupacionExistencia.Material
                : request.AgruparPor.Trim().ToUpperInvariant();

            // Un movimiento sin tropa (por ejemplo un egreso futuro del Ciclo II) no tiene dueno:
            // el left join lo deja sin cliente en lugar de descartarlo del saldo.
            var baseQuery =
                from mc in this.context.MovimientosCamaras
                join a in this.context.Almacenes on mc.AlmacenId equals a.Id
                join est in this.context.Establecimientos on a.EstablecimientoId equals est.Id
                join m in this.context.Materiales on mc.MaterialId equals m.Id
                join tm in this.context.TiposMateriales on m.TipoMaterialId equals tm.Codigo into tmj
                from tm in tmj.DefaultIfEmpty()
                join t in this.context.Tropas on mc.TropaId equals t.Id into tj
                from t in tj.DefaultIfEmpty()
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id into ij
                from i in ij.DefaultIfEmpty()
                join c in this.context.Clientes on i.ClienteId equals c.Id into cj
                from c in cj.DefaultIfEmpty()
                where (request.EstablecimientoId == null || est.Id == request.EstablecimientoId)
                    && (request.AlmacenId == null || a.Id == request.AlmacenId)
                    && (request.MaterialId == null || m.Id == request.MaterialId)
                    && (request.ClienteId == null || (c != null && c.Id == request.ClienteId))
                select new
                {
                    mc.Cantidad,
                    mc.Peso,
                    mc.Fecha,
                    AlmacenId = a.Id,
                    AlmacenNombre = a.Nombre,
                    MaterialId = m.Id,
                    m.CodigoMaterial,
                    MaterialNombre = m.Nombre,
                    m.TipoMaterialId,
                    TipoMaterialNombre = tm != null ? tm.Nombre : null,
                    ClienteId = c != null ? (Guid?)c.Id : null,
                    ClienteNombre = c != null ? c.Nombre : null
                };

            var filas = await baseQuery.ToListAsync(cancellationToken);

            // La agrupacion se hace en memoria: el saldo de una camara es de decenas de lineas,
            // no de millones, y asi los tres cortes comparten una sola consulta.
            List<ExistenciaCamaraItem> data;
            switch (agrupacion)
            {
                case AgrupacionExistencia.Proveedor:
                    data = filas
                        .GroupBy(x => new { x.ClienteId, x.ClienteNombre, x.MaterialId, x.CodigoMaterial, x.MaterialNombre, x.TipoMaterialId, x.TipoMaterialNombre })
                        .Select(g => new ExistenciaCamaraItem
                        {
                            ClienteId = g.Key.ClienteId,
                            ClienteNombre = g.Key.ClienteNombre,
                            MaterialId = g.Key.MaterialId,
                            MaterialCodigo = g.Key.CodigoMaterial,
                            MaterialNombre = g.Key.MaterialNombre,
                            TipoMaterialId = g.Key.TipoMaterialId,
                            TipoMaterialNombre = g.Key.TipoMaterialNombre,
                            Cantidad = g.Sum(x => x.Cantidad),
                            Peso = g.Sum(x => x.Peso),
                            UltimoMovimiento = g.Max(x => (DateTime?)x.Fecha)
                        })
                        .OrderBy(x => x.ClienteNombre).ThenBy(x => x.MaterialNombre)
                        .ToList();
                    break;

                case AgrupacionExistencia.Camara:
                    data = filas
                        .GroupBy(x => new { x.AlmacenId, x.AlmacenNombre, x.ClienteId, x.ClienteNombre })
                        .Select(g => new ExistenciaCamaraItem
                        {
                            AlmacenId = g.Key.AlmacenId,
                            AlmacenNombre = g.Key.AlmacenNombre,
                            ClienteId = g.Key.ClienteId,
                            ClienteNombre = g.Key.ClienteNombre,
                            Cantidad = g.Sum(x => x.Cantidad),
                            Peso = g.Sum(x => x.Peso),
                            UltimoMovimiento = g.Max(x => (DateTime?)x.Fecha)
                        })
                        .OrderBy(x => x.AlmacenNombre).ThenBy(x => x.ClienteNombre)
                        .ToList();
                    break;

                default: // MATERIAL: la vista de inventario, el corte por defecto.
                    data = filas
                        .GroupBy(x => new { x.AlmacenId, x.AlmacenNombre, x.MaterialId, x.CodigoMaterial, x.MaterialNombre, x.TipoMaterialId, x.TipoMaterialNombre })
                        .Select(g => new ExistenciaCamaraItem
                        {
                            AlmacenId = g.Key.AlmacenId,
                            AlmacenNombre = g.Key.AlmacenNombre,
                            MaterialId = g.Key.MaterialId,
                            MaterialCodigo = g.Key.CodigoMaterial,
                            MaterialNombre = g.Key.MaterialNombre,
                            TipoMaterialId = g.Key.TipoMaterialId,
                            TipoMaterialNombre = g.Key.TipoMaterialNombre,
                            Cantidad = g.Sum(x => x.Cantidad),
                            Peso = g.Sum(x => x.Peso),
                            UltimoMovimiento = g.Max(x => (DateTime?)x.Fecha)
                        })
                        .OrderBy(x => x.AlmacenNombre).ThenBy(x => x.MaterialNombre)
                        .ToList();
                    agrupacion = AgrupacionExistencia.Material;
                    break;
            }

            // Una linea en cero es un material que entro y se transformo por completo (la media
            // res cuarteada). No es existencia, asi que por defecto no se muestra.
            if (!request.IncluirSaldoCero)
                data = data.Where(x => x.Cantidad != 0).ToList();

            return new GetExistenciaCamaraResponse
            {
                Data = data,
                AgruparPor = agrupacion,
                TotalCantidad = data.Sum(x => x.Cantidad),
                TotalPeso = data.Sum(x => x.Peso)
            };
        }
    }
}
