using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Sucursales.DeleteSucursal
{

    public class DeleteSucursalHandler : IRequestHandler<DeleteSucursalRequest, DeleteSucursalResponse>
    {
        private readonly MeatContext context;

        public DeleteSucursalHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteSucursalResponse> Handle(DeleteSucursalRequest request, CancellationToken cancellationToken)
        {
            //var articulosSucursal = await this.context.Articulos.Where(x => x.SucursalId == request.Id).ToListAsync();
            //this.context.Articulos.RemoveRange(articulosSucursal);

            var sucursal = await this.context.Sucursales
                .Include(x => x.Puestos)
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            if (sucursal == null)
            {
                throw new ValidationException("La sucursal no existe");
            }

            var tieneEstablecimientos = await this.context.Establecimientos.AnyAsync(e => e.SucursalId == request.Id, cancellationToken);
            if (tieneEstablecimientos)
                throw new ValidationException("No se puede eliminar la sucursal porque tiene establecimientos asignados.");

            var tieneUsuarios = await this.context.UsuariosSucursales.AnyAsync(us => us.SucursalId == request.Id, cancellationToken);
            if (tieneUsuarios)
                throw new ValidationException("No se puede eliminar la sucursal porque tiene usuarios asignados.");

            this.context.Sucursales.Remove(sucursal);

            await this.context.SaveChangesAsync();

            return new DeleteSucursalResponse();
        }
    }
}
