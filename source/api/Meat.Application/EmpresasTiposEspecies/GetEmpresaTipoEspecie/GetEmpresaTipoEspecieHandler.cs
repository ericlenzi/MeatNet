using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.EmpresasTiposEspecies.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EmpresasTiposEspecies.GetEmpresaTipoEspecie
{
    public class GetEmpresaTipoEspecieHandler : IRequestHandler<GetEmpresaTipoEspecieRequest, GetEmpresaTipoEspecieResponse>
    {
        private readonly MeatContext context;

        public GetEmpresaTipoEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEmpresaTipoEspecieResponse> Handle(GetEmpresaTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var item = await this.context.EmpresasTiposEspecies
                .Where(x => x.Id == request.Id)
                .Select(EmpresaTipoEspecieProyeccion.Item)
                .FirstOrDefaultAsync(cancellationToken);

            if (item == null)
                return null;

            return new GetEmpresaTipoEspecieResponse
            {
                Id = item.Id,
                TipoEspecieId = item.TipoEspecieId,
                Nombre = item.Nombre,
                EspecieId = item.EspecieId,
                EspecieNombre = item.EspecieNombre,
                TipoSexoId = item.TipoSexoId,
                TipoSexoNombre = item.TipoSexoNombre,
                PesoTeorico = item.PesoTeorico,
                PesoTeoricoReferencia = item.PesoTeoricoReferencia,
                ERP_Codigo = item.ERP_Codigo,
                Activo = item.Activo,
                TipoEspecieActivo = item.TipoEspecieActivo,
            };
        }
    }
}
