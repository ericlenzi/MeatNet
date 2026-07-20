using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores.GetNumeradores
{
    public class GetNumeradoresHandler : IRequestHandler<GetNumeradoresRequest, GetNumeradoresResponse>
    {
        private readonly MeatContext context;

        public GetNumeradoresHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetNumeradoresResponse> Handle(GetNumeradoresRequest request, CancellationToken cancellationToken)
        {
            var data = await (
                from n in this.context.Numeradores
                join e in this.context.Establecimientos on n.EstablecimientoId equals e.Id
                join emp in this.context.Empresas on e.EmpresaId equals emp.Id
                join esp in this.context.Especies on n.EspecieCodigo equals esp.Codigo into espj
                from esp in espj.DefaultIfEmpty()
                where emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || n.EstablecimientoId == request.EstablecimientoId)
                    && (request.Estado == null || n.Activo == request.Estado)
                orderby e.Nombre, n.Codigo
                select new NumeradorItem
                {
                    Id = n.Id,
                    EstablecimientoId = n.EstablecimientoId,
                    EstablecimientoNombre = e.Nombre,
                    EspecieCodigo = n.EspecieCodigo,
                    EspecieNombre = esp != null ? esp.Nombre : null,
                    Codigo = n.Codigo,
                    Descripcion = n.Descripcion,
                    TipoNumerador = n.TipoNumerador,
                    UltimoNumero = n.UltimoNumero,
                    Activo = n.Activo
                }
            ).ToListAsync(cancellationToken);

            return new GetNumeradoresResponse { Data = data };
        }
    }
}
