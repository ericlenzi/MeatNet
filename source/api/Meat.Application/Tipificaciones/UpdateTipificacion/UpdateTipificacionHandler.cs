using MediatR;
using Meat.Application.Shared;
using Meat.Application.Tipificaciones.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.UpdateTipificacion
{
    public class UpdateTipificacionHandler : IRequestHandler<UpdateTipificacionRequest, UpdateTipificacionResponse>
    {
        private readonly MeatContext context;

        public UpdateTipificacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateTipificacionResponse> Handle(UpdateTipificacionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Tipificaciones
                .FirstOrDefaultAsync(t => t.Codigo == request.Codigo && t.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (entity == null)
                throw new ValidationException("La tipificacion no existe.");

            await TipificacionValidacion.ValidateAsync(
                this.context, request.EspecieId, request.TipoEspecieId, request.UnidadFaenaId,
                request.DestinoComercialId, request.TipificacionOficialId, request.UnidadMedidaId,
                request.PesoDesde, request.PesoHasta, cancellationToken);

            entity.Descripcion = request.Descripcion;
            entity.EspecieId = request.EspecieId;
            entity.TipoEspecieId = request.TipoEspecieId;
            entity.UnidadFaenaId = request.UnidadFaenaId;
            entity.DestinoComercialId = request.DestinoComercialId;
            entity.TipificacionOficialId = request.TipificacionOficialId;
            entity.PesoDesde = request.PesoDesde;
            entity.PesoHasta = request.PesoHasta;
            entity.UnidadMedidaId = request.UnidadMedidaId;
            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipificacionResponse();
        }
    }
}
