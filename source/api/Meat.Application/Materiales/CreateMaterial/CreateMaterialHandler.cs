using MediatR;
using Meat.Application.Shared;
using Meat.Domain.Materiales;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Materiales.CreateMaterial
{
    public class CreateMaterialHandler : IRequestHandler<CreateMaterialRequest, CreateMaterialResponse>
    {
        private readonly MeatContext context;

        public CreateMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateMaterialResponse> Handle(CreateMaterialRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CodigoMaterial))
                throw new ValidationException("El codigo de material es requerido.");
            var codigo = request.CodigoMaterial.Trim();

            var codigoEnUso = await this.context.Materiales
                .AnyAsync(m => m.CodigoMaterial == codigo, cancellationToken);
            if (codigoEnUso)
                throw new ValidationException("Ya existe un material con ese codigo.");

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

            var entity = MaterialFactory.Create();
            entity.CodigoMaterial = codigo;
            entity.Nombre = request.Nombre;
            entity.TipoMaterialId = request.TipoMaterialId;
            entity.UnidadMedidaId = request.UnidadMedidaId;
            entity.PesoTeorico = request.PesoTeorico;
            entity.ERP_Codigo = request.ERP_Codigo;
            entity.FechaActualizacion = DateTime.Now;

            this.context.Materiales.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateMaterialResponse { Id = entity.Id };
        }
    }
}
