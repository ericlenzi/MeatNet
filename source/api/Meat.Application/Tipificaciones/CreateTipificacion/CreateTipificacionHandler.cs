using MediatR;
using Meat.Application.Shared;
using Meat.Application.Tipificaciones.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.CreateTipificacion
{
    public class CreateTipificacionHandler : IRequestHandler<CreateTipificacionRequest, CreateTipificacionResponse>
    {
        private readonly MeatContext context;

        public CreateTipificacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateTipificacionResponse> Handle(CreateTipificacionRequest request, CancellationToken cancellationToken)
        {
            // El codigo es el identificador que teclea el usuario: se recorta antes de buscar el
            // duplicado, si no " 3411" y "3411" conviven como codigos distintos.
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            var yaExiste = await this.context.Tipificaciones.AnyAsync(t => t.Codigo == codigo, cancellationToken);
            if (yaExiste)
                throw new ValidationException("Ya existe una tipificacion con ese codigo.");

            await TipificacionValidacion.ValidateAsync(
                this.context, request.EspecieId, request.TipoEspecieId, request.UnidadFaenaId ?? Guid.Empty,
                request.DestinoComercialId, request.TipificacionOficialId, request.UnidadMedidaId,
                request.PesoDesde, request.PesoHasta, request.MaterialId, cancellationToken);

            var entity = Domain.Tipificaciones.TipificacionFactory.Create();
            entity.Codigo = codigo;
            entity.Descripcion = request.Descripcion?.Trim();
            entity.EmpresaId = request.EmpresaId;
            entity.EspecieId = request.EspecieId;
            entity.TipoEspecieId = request.TipoEspecieId;
            entity.UnidadFaenaId = request.UnidadFaenaId.Value;
            entity.DestinoComercialId = request.DestinoComercialId;
            entity.TipificacionOficialId = request.TipificacionOficialId;
            entity.PesoDesde = request.PesoDesde;
            entity.PesoHasta = request.PesoHasta;
            entity.UnidadMedidaId = request.UnidadMedidaId;
            entity.MaterialId = request.MaterialId;
            entity.Puntos = 0;

            this.context.Tipificaciones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipificacionResponse { Id = entity.Id };
        }
    }
}
