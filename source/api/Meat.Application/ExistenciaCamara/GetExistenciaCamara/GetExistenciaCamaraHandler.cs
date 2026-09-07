using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ExistenciaCamara.GetExistenciaCamara
{
    /// <summary>
    /// Saldo de existencia de camara por (camara, material), derivado del log MovimientosCamaras
    /// sumando Cantidad y Peso. Mismo patron que el En Pie derivado: el log es la fuente de verdad
    /// y el saldo se calcula, no se guarda. Ver docs/manuales/EvaluacionFaena.md.
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
            var baseQuery =
                from mc in this.context.MovimientosCamaras
                join a in this.context.Almacenes on mc.AlmacenId equals a.Id
                join est in this.context.Establecimientos on a.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                join m in this.context.Materiales on mc.MaterialId equals m.Id
                join tm in this.context.TiposMateriales on m.TipoMaterialId equals tm.Codigo into tmj
                from tm in tmj.DefaultIfEmpty()
                where emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || est.Id == request.EstablecimientoId)
                    && (request.AlmacenId == null || a.Id == request.AlmacenId)
                    && (request.MaterialId == null || m.Id == request.MaterialId)
                select new { mc, a, m, tm };

            var data = await baseQuery
                .GroupBy(x => new
                {
                    AlmacenId = x.a.Id,
                    AlmacenNombre = x.a.Nombre,
                    MaterialId = x.m.Id,
                    x.m.CodigoMaterial,
                    MaterialNombre = x.m.Nombre,
                    x.m.TipoMaterialId,
                    TipoMaterialNombre = x.tm.Nombre
                })
                .Select(g => new ExistenciaCamaraItem
                {
                    AlmacenId = g.Key.AlmacenId,
                    AlmacenNombre = g.Key.AlmacenNombre,
                    MaterialId = g.Key.MaterialId,
                    MaterialCodigo = g.Key.CodigoMaterial,
                    MaterialNombre = g.Key.MaterialNombre,
                    TipoMaterialId = g.Key.TipoMaterialId,
                    TipoMaterialNombre = g.Key.TipoMaterialNombre,
                    Cantidad = g.Sum(x => x.mc.Cantidad),
                    Peso = g.Sum(x => x.mc.Peso),
                    UltimoMovimiento = g.Max(x => (System.DateTime?)x.mc.Fecha)
                })
                .ToListAsync(cancellationToken);

            // Una linea en cero es un material que entro y se transformo por completo (la media
            // res cuarteada). No es existencia, asi que por defecto no se muestra.
            if (!request.IncluirSaldoCero)
                data = data.Where(x => x.Cantidad != 0).ToList();

            return new GetExistenciaCamaraResponse
            {
                Data = data
                    .OrderBy(x => x.AlmacenNombre)
                    .ThenBy(x => x.MaterialNombre)
                    .ToList(),
                TotalCantidad = data.Sum(x => x.Cantidad),
                TotalPeso = data.Sum(x => x.Peso)
            };
        }
    }
}
