using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.EnviarAprobacionIngresoHacienda
{
    public class EnviarAprobacionIngresoHaciendaHandler : IRequestHandler<EnviarAprobacionIngresoHaciendaRequest, EnviarAprobacionIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public EnviarAprobacionIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<EnviarAprobacionIngresoHaciendaResponse> Handle(EnviarAprobacionIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Ubicaciones)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.Borrador)
                throw new ValidationException("Solo se puede enviar a aprobacion un ingreso en estado Borrador.");

            if (entity.Ubicaciones == null || !entity.Ubicaciones.Any())
                throw new ValidationException("El ingreso no tiene ubicaciones en corral cargadas.");

            entity.EstadoIngresoId = EstadosIngreso.PendienteAprobacion;
            entity.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync(cancellationToken);

            return new EnviarAprobacionIngresoHaciendaResponse();
        }
    }
}
