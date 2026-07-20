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
            var yaExiste = await this.context.Tipificaciones.AnyAsync(t => t.Codigo == request.Codigo, cancellationToken);
            if (yaExiste)
                throw new ValidationException("Ya existe una tipificacion con ese codigo.");

            await TipificacionValidacion.ValidateAsync(
                this.context, request.EspecieId, request.TipoEspecieId, request.UnidadFaenaId,
                request.DestinoComercialId, request.TipificacionOficialId, request.UnidadMedidaId,
                request.PesoDesde, request.PesoHasta, cancellationToken);

            var entity = new Domain.Tipificaciones.Tipificacion
            {
                Codigo = request.Codigo,
                Descripcion = request.Descripcion,
                CodigoEmpresa = request.CodigoEmpresa,
                EspecieId = request.EspecieId,
                TipoEspecieId = request.TipoEspecieId,
                UnidadFaenaId = request.UnidadFaenaId,
                DestinoComercialId = request.DestinoComercialId,
                TipificacionOficialId = request.TipificacionOficialId,
                PesoDesde = request.PesoDesde,
                PesoHasta = request.PesoHasta,
                UnidadMedidaId = request.UnidadMedidaId,
                Puntos = 0,
                Activo = true,
                FechaActualizacion = DateTime.Now
            };

            this.context.Tipificaciones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipificacionResponse { Codigo = entity.Codigo };
        }
    }
}
