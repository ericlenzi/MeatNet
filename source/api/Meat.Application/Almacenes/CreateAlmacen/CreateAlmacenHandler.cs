using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Almacenes.CreateAlmacen
{
    public class CreateAlmacenHandler : IRequestHandler<CreateAlmacenRequest, CreateAlmacenResponse>
    {
        private readonly MeatContext context;

        public CreateAlmacenHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateAlmacenResponse> Handle(CreateAlmacenRequest request, CancellationToken cancellationToken)
        {
            var establecimiento = await this.context.Establecimientos
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId
                    && e.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (establecimiento == null)
                throw new ValidationException("El establecimiento activo no es valido.");

            var existe = await this.context.Almacenes
                .AnyAsync(a => a.EstablecimientoId == request.EstablecimientoId
                    && a.CodigoAlmacen == request.CodigoAlmacen, cancellationToken);
            if (existe)
                throw new ValidationException("Ya existe un corral con ese codigo en el establecimiento.");

            var entity = new Domain.Almacenes.Almacen
            {
                Id = Guid.NewGuid(),
                CodigoAlmacen = request.CodigoAlmacen,
                Nombre = request.Nombre,
                CantidadAnimales = request.CantidadAnimales,
                TipoAlmacenId = request.TipoAlmacenId,
                ERP_Codigo = request.ERP_Codigo,
                EstablecimientoId = request.EstablecimientoId,
                Activo = true,
                FechaActualizacion = DateTime.Now
            };

            this.context.Almacenes.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateAlmacenResponse { Id = entity.Id };
        }
    }
}
