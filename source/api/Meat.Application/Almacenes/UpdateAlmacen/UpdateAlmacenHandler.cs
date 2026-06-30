using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Almacenes.UpdateAlmacen
{
    public class UpdateAlmacenHandler : IRequestHandler<UpdateAlmacenRequest, UpdateAlmacenResponse>
    {
        private readonly MeatContext context;

        public UpdateAlmacenHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateAlmacenResponse> Handle(UpdateAlmacenRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Almacenes
                .Include(a => a.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(a => a.Id == request.Id
                    && a.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El corral no existe.");

            entity.Nombre = request.Nombre;
            entity.CantidadAnimales = request.CantidadAnimales;
            entity.TipoAlmacenId = request.TipoAlmacenId;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateAlmacenResponse();
        }
    }
}
