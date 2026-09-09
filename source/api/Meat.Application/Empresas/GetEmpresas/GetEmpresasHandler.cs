using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.Empresas;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.GetEmpresas
{
    public class GetEmpresasHandler : IRequestHandler<GetEmpresasRequest, GetEmpresasResponse>
    {
        private readonly MeatContext context;

        public GetEmpresasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEmpresasResponse> Handle(GetEmpresasRequest request, CancellationToken cancellationToken)
        {
            IQueryable<Empresa> queryable = this.context.Empresas.AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.Nombre.Contains(request.Filter) ||
                    x.Id.Contains(request.Filter) ||
                    x.NumeroCuit.Contains(request.Filter));

            if (request.Estado.HasValue)
                queryable = queryable.Where(x => x.Activo == request.Estado.Value);

            queryable = queryable.OrderBy(x => x.Id);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable
                .Page(request.PageSize, request.PageIndex)
                .Select(x => new EmpresaItem
                {
                    Id = x.Id,
                    Nombre = x.Nombre,
                    TipoEmpresaId = x.TipoEmpresaId,
                    NumeroCuit = x.NumeroCuit,
                    NumeroIngresosBrutos = x.NumeroIngresosBrutos,
                    NumeroInscripcionRuca = x.NumeroInscripcionRuca,
                    CodigoActividad = x.CodigoActividad,
                    ERP_Codigo = x.ERP_Codigo,
                    Color = x.Color,
                    Activo = x.Activo
                })
                .ToListAsync(cancellationToken);

            return new GetEmpresasResponse()
            {
                Data = data,
                TotalRows = totalRows
            };
        }
    }
}
