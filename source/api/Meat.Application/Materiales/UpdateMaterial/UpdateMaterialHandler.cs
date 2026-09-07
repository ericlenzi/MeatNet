using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Materiales.UpdateMaterial
{
    public class UpdateMaterialHandler : IRequestHandler<UpdateMaterialRequest, UpdateMaterialResponse>
    {
        private readonly MeatContext context;

        public UpdateMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateMaterialResponse> Handle(UpdateMaterialRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Materiales
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El material no existe.");

            if (!string.IsNullOrEmpty(request.TipoMaterialId))
            {
                var tipoExiste = await this.context.TiposMateriales
                    .AnyAsync(t => t.Codigo == request.TipoMaterialId, cancellationToken);
                if (!tipoExiste)
                    throw new ValidationException("El tipo de material indicado no existe.");
            }

            if (!string.IsNullOrEmpty(request.UnidadMedidaId))
            {
                var umExiste = await this.context.UnidadesMedidas
                    .AnyAsync(u => u.Codigo == request.UnidadMedidaId, cancellationToken);
                if (!umExiste)
                    throw new ValidationException("La unidad de medida indicada no existe.");
            }

            entity.Nombre = request.Nombre;
            entity.TipoMaterialId = request.TipoMaterialId;
            entity.UnidadMedidaId = request.UnidadMedidaId;
            entity.PesoTeorico = request.PesoTeorico;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateMaterialResponse();
        }
    }
}
