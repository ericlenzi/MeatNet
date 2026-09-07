using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ExistenciaCamara.GetMovimientosCamara
{
    /// <summary>
    /// Movimientos que componen la existencia de camara: el detalle detras del saldo. Cada linea
    /// se remonta a la media res y al garron que la originaron, que es la trazabilidad que promete
    /// el paso 4 (R-L5). Sirve tanto para auditar un saldo como para responder "de donde salio
    /// este cuarto".
    /// </summary>
    public class GetMovimientosCamaraHandler : IRequestHandler<GetMovimientosCamaraRequest, GetMovimientosCamaraResponse>
    {
        private readonly MeatContext context;

        public GetMovimientosCamaraHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMovimientosCamaraResponse> Handle(GetMovimientosCamaraRequest request, CancellationToken cancellationToken)
        {
            var query =
                from mc in this.context.MovimientosCamaras
                join a in this.context.Almacenes on mc.AlmacenId equals a.Id
                join est in this.context.Establecimientos on a.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                join m in this.context.Materiales on mc.MaterialId equals m.Id
                join tmc in this.context.TiposMovimientosCamaras on mc.TipoMovimientoId equals tmc.Codigo into tmcj
                from tmc in tmcj.DefaultIfEmpty()
                where emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.AlmacenId == null || a.Id == request.AlmacenId)
                    && (request.MaterialId == null || m.Id == request.MaterialId)
                    && (request.TropaId == null || mc.TropaId == request.TropaId)
                    && (request.RomaneoPiezaOrigenId == null || mc.RomaneoPiezaOrigenId == request.RomaneoPiezaOrigenId)
                    && (request.FechaDesde == null || mc.Fecha >= request.FechaDesde)
                    && (request.FechaHasta == null || mc.Fecha <= request.FechaHasta)
                select new MovimientoCamaraItem
                {
                    Id = mc.Id,
                    Fecha = mc.Fecha,
                    TipoMovimientoId = mc.TipoMovimientoId,
                    TipoMovimientoNombre = tmc.Nombre,
                    AlmacenId = a.Id,
                    AlmacenNombre = a.Nombre,
                    MaterialId = m.Id,
                    MaterialCodigo = m.CodigoMaterial,
                    MaterialNombre = m.Nombre,
                    Cantidad = mc.Cantidad,
                    Peso = mc.Peso,
                    TransformacionId = mc.TransformacionId,
                    RomaneoPiezaOrigenId = mc.RomaneoPiezaOrigenId,
                    // Navegaciones opcionales: un movimiento puede no venir de un romaneo
                    // (por ejemplo un egreso futuro del Ciclo II).
                    NumeroRomaneo = mc.RomaneoPiezaOrigen.Romaneo.NumeroRomaneo,
                    NumeroGarron = mc.RomaneoPiezaOrigen.Romaneo.NumeroGarron,
                    Letra = mc.RomaneoPiezaOrigen.Letra,
                    TropaId = mc.TropaId,
                    NumeroTropa = mc.Tropa.NumeroTropa,
                    EspecieId = mc.EspecieId,
                    TipoEspecieId = mc.TipoEspecieId,
                    Referencia = mc.Referencia
                };

            var data = await query
                .OrderByDescending(x => x.Fecha)
                .ThenBy(x => x.NumeroRomaneo)
                .ToListAsync(cancellationToken);

            return new GetMovimientosCamaraResponse { Data = data };
        }
    }
}
