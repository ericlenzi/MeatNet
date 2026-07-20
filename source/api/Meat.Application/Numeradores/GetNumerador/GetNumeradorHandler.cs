using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores.GetNumerador
{
    public class GetNumeradorHandler : IRequestHandler<GetNumeradorRequest, GetNumeradorResponse>
    {
        private readonly MeatContext context;

        public GetNumeradorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetNumeradorResponse> Handle(GetNumeradorRequest request, CancellationToken cancellationToken)
        {
            return await (
                from n in this.context.Numeradores
                join e in this.context.Establecimientos on n.EstablecimientoId equals e.Id
                join emp in this.context.Empresas on e.EmpresaId equals emp.Id
                join esp in this.context.Especies on n.EspecieCodigo equals esp.Codigo into espj
                from esp in espj.DefaultIfEmpty()
                where n.Id == request.Id && emp.CodigoEmpresa == request.CodigoEmpresa
                select new GetNumeradorResponse
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
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
