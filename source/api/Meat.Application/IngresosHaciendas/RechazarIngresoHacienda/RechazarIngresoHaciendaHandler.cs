using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.RechazarIngresoHacienda
{
    public class RechazarIngresoHaciendaHandler : IRequestHandler<RechazarIngresoHaciendaRequest, RechazarIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public RechazarIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<RechazarIngresoHaciendaResponse> Handle(RechazarIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.PendienteAprobacion)
                throw new ValidationException("Solo se puede rechazar un ingreso en estado Pendiente Aprobacion.");

            entity.EstadoIngresoId = EstadosIngreso.Borrador;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new RechazarIngresoHaciendaResponse();
        }
    }
}
