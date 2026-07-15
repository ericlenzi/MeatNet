using MediatR;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.IniciarListaMatanza
{
    public class IniciarListaMatanzaHandler : IRequestHandler<IniciarListaMatanzaRequest, IniciarListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public IniciarListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<IniciarListaMatanzaResponse> Handle(IniciarListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Confirmada)
                throw new ValidationException("Solo se puede iniciar la ejecucion de una lista Confirmada.");

            entity.EstadoListaMatanzaId = EstadosListaMatanza.EnEjecucion;
            entity.Version += 1;
            entity.FechaInicioEjecucion = DateTime.Now;
            entity.FechaActualizacion = DateTime.Now;

            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = entity.Id,
                Version = entity.Version,
                Fecha = DateTime.Now,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.Inicio,
                Motivo = "Inicio de ejecucion de la faena."
            });

            await this.context.SaveChangesAsync(cancellationToken);

            return new IniciarListaMatanzaResponse();
        }
    }
}
