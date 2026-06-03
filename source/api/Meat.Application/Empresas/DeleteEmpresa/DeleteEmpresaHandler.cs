using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.DeleteEmpresa
{
    public class DeleteEmpresaHandler : IRequestHandler<DeleteEmpresaRequest, DeleteEmpresaResponse>
    {
        private readonly MeatContext context;

        public DeleteEmpresaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEmpresaResponse> Handle(DeleteEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (empresa == null)
                throw new ValidationException("La empresa no existe");

            var tieneSucursales = await this.context.Sucursales.AnyAsync(x => x.EmpresaId == request.Id, cancellationToken);
            if (tieneSucursales)
                throw new ValidationException("No se puede eliminar la empresa porque tiene sucursales asignadas.");

            var tieneEstablecimientos = await this.context.Establecimientos.AnyAsync(x => x.EmpresaId == request.Id, cancellationToken);
            if (tieneEstablecimientos)
                throw new ValidationException("No se puede eliminar la empresa porque tiene establecimientos asignados.");

            this.context.Empresas.Remove(empresa);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEmpresaResponse();
        }
    }
}
