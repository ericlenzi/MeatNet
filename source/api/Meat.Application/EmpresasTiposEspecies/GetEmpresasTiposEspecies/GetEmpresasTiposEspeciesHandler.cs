using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.EmpresasTiposEspecies.Shared;
using Meat.Application.Shared;
using Meat.Domain.EmpresasTiposEspecies;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EmpresasTiposEspecies.GetEmpresasTiposEspecies
{
    /// <summary>
    /// Categorias con las que opera la empresa activa. Es la fuente de los combos operativos:
    /// el catalogo global (TiposEspecies) lista todas las del rubro, esto lista las suyas.
    /// </summary>
    public class GetEmpresasTiposEspeciesHandler : IRequestHandler<GetEmpresasTiposEspeciesRequest, GetEmpresasTiposEspeciesResponse>
    {
        private readonly MeatContext context;

        public GetEmpresasTiposEspeciesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEmpresasTiposEspeciesResponse> Handle(GetEmpresasTiposEspeciesRequest request, CancellationToken cancellationToken)
        {
            IQueryable<EmpresaTipoEspecie> queryable = this.context.EmpresasTiposEspecies;

            if (!string.IsNullOrEmpty(request.EspecieId))
                queryable = queryable.Where(x => x.TipoEspecie.EspecieId == request.EspecieId);

            if (request.Estado.HasValue)
            {
                // El estado efectivo necesita las dos puntas: una categoria dada de baja en el
                // catalogo no se puede usar aunque la empresa la tenga activa.
                var activo = request.Estado.Value;
                queryable = queryable.Where(x => (x.Activo && x.TipoEspecie.Activo) == activo);
            }

            if (!string.IsNullOrEmpty(request.Filter))
                queryable = queryable.Where(x =>
                    x.TipoEspecieId.Contains(request.Filter) ||
                    x.TipoEspecie.Nombre.Contains(request.Filter));

            queryable = queryable
                .OrderBy(x => x.TipoEspecie.EspecieId)
                .ThenBy(x => x.TipoEspecie.Nombre);

            var totalRows = await queryable.CountAsync(cancellationToken);

            var data = await queryable
                .Page(request.PageSize, request.PageIndex)
                .Select(EmpresaTipoEspecieProyeccion.Item)
                .ToListAsync(cancellationToken);

            return new GetEmpresasTiposEspeciesResponse
            {
                Data = data,
                TotalRows = totalRows,
            };
        }
    }
}
