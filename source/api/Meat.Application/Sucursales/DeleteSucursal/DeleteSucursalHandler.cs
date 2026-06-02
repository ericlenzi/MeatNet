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
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (sucursal == null)
            {
                throw new ValidationException("La sucursal no existe");
            }

            this.context.Sucursales.Remove(sucursal);

            await this.context.SaveChangesAsync();

            return new DeleteSucursalResponse();
        }
    }
}
