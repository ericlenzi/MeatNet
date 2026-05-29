using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ParametrosSucursales.UpdateParametroSucursal
{
    public class UpdateParametroSucursalHandler : IRequestHandler<UpdateParametroSucursalRequest, UpdateParametroSucursalResponse>
    {
        private readonly MeatContext context;

        public UpdateParametroSucursalHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateParametroSucursalResponse> Handle(UpdateParametroSucursalRequest request, CancellationToken cancellationToken)
        {
            var parametroSucursal = await this.context.ParametrosSucursales.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (parametroSucursal == null)
            {
                throw new ValidationException("El parametro no existe");
            }

            parametroSucursal.Valor = request.Valor;
            parametroSucursal.FechaActualizacion = System.DateTime.Now;
            context.ParametrosSucursales.Update(parametroSucursal);

            await this.context.SaveChangesAsync();

            return new UpdateParametroSucursalResponse();
        }
    }
}
