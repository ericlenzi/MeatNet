using MediatR;
using Meat.Application.EvaluacionFaena.Shared;
using Meat.Application.ListasMatanzas;
using Meat.Application.MovimientosCamaras;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Domain.MovimientosCamaras;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EvaluacionFaena.LiberarJornada
{
    /// <summary>
    /// Libera la jornada (Ciclo I paso 4): materializa la existencia de camara a partir de los
    /// romaneos y deja los romaneos definitivos. Es el cierre del Ciclo I: a partir de aca el
    /// inventario deja de contarse por animal (TipoEspecie, en pie) y pasa a contarse por
    /// producto (Material, en camara), listo para el Ciclo II.
    ///
    /// Es todo o nada: si una sola pieza no se puede liberar no se escribe ningun movimiento y
    /// la jornada queda intacta. Los problemas se ven antes con PrevisualizarLiberacion.
    /// Ver docs/manuales/EvaluacionFaena.md.
    /// </summary>
    public class LiberarJornadaHandler : IRequestHandler<LiberarJornadaRequest, LiberarJornadaResponse>
    {
        /// <summary>Cuantos problemas se detallan en el mensaje de error antes de resumir el resto.</summary>
        private const int MaxProblemasEnMensaje = 5;

        private readonly MeatContext context;

        public LiberarJornadaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<LiberarJornadaResponse> Handle(LiberarJornadaRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento).ThenInclude(e => e.Empresa)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId
                    && x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            // O-1: solo se libera con la jornada cerrada. El sobrante no faenado ya se libero
            // del reservado al finalizar, asi que lo que queda es lo efectivamente faenado.
            if (lm.EstadoListaMatanzaId != EstadosListaMatanza.Finalizada)
                throw new ValidationException("Solo se puede liberar una lista Finalizada. Cierre la jornada antes de liberar.");

            var plan = await LiberacionCalculo.CalcularAsync(
                this.context, lm.Id, lm.EstablecimientoId, lm.NumeroLista.ToString(), cancellationToken);

            if (plan.PiezasAProcesar == 0)
            {
                // R-L7: liberar dos veces no duplica existencia.
                if (plan.PiezasYaLiberadas > 0)
                    throw new ValidationException("La jornada ya fue liberada.");
                throw new ValidationException("La jornada no tiene romaneos para liberar.");
            }

            // Todo o nada: un problema aborta la liberacion completa.
            if (plan.TieneProblemas)
                throw new ValidationException(ArmarMensajeProblemas(plan));

            await using var tx = await this.context.Database.BeginTransactionAsync(cancellationToken);

            var ahora = DateTime.Now;

            foreach (var m in plan.Movimientos)
            {
                var movimiento = MovimientoCamaraFactory.Create();
                movimiento.Fecha = ahora;
                movimiento.AlmacenId = m.AlmacenId;
                movimiento.MaterialId = m.MaterialId;
                movimiento.TipoMovimientoId = m.TipoMovimientoId;
                movimiento.Cantidad = m.Cantidad;
                movimiento.Peso = m.Peso;
                movimiento.RomaneoPiezaOrigenId = m.PiezaId;
                movimiento.TransformacionId = m.TransformacionId;
                movimiento.TropaId = m.TropaId;
                movimiento.EspecieId = m.EspecieId;
                movimiento.TipoEspecieId = m.TipoEspecieId;
                movimiento.UsuarioId = request.UsuarioId;
                movimiento.Referencia = m.Referencia;

                this.context.MovimientosCamaras.Add(movimiento);
            }

            // R-L3: las piezas liberadas quedan inmutables.
            var piezasIds = plan.Movimientos.Select(m => m.PiezaId).Distinct().ToList();
            var piezas = await this.context.RomaneosPiezas
                .Where(p => piezasIds.Contains(p.Id))
                .ToListAsync(cancellationToken);
            foreach (var pieza in piezas)
                pieza.Liberado = true;

            // El romaneo queda definitivo cuando todas sus piezas lo estan.
            var romaneosIds = plan.Movimientos.Select(m => m.RomaneoId).Distinct().ToList();
            var romaneos = await this.context.Romaneos
                .Include(r => r.Piezas)
                .Where(r => romaneosIds.Contains(r.Id))
                .ToListAsync(cancellationToken);
            var romaneosLiberados = 0;
            foreach (var romaneo in romaneos)
            {
                if (!romaneo.Piezas.All(p => p.Liberado)) continue;

                romaneo.Liberado = true;
                romaneo.FechaLiberacion = ahora;
                romaneo.UsuarioLiberacionId = request.UsuarioId;
                romaneosLiberados += 1;
            }

            var kilosIngresados = plan.Movimientos.Where(m => m.Cantidad > 0).Sum(m => m.Peso);

            // Deja el hito en el historial de la LM, donde el usuario ya sigue la vida de la jornada.
            this.context.ListasMatanzasMovimientos.Add(new ListaMatanzaMovimiento
            {
                Id = Guid.NewGuid(),
                ListaMatanzaId = lm.Id,
                Version = lm.Version,
                Fecha = ahora,
                UsuarioId = request.UsuarioId,
                TipoMovimiento = TiposMovimientoLM.LiberacionCamara,
                Motivo = $"Liberacion: {piezas.Count} piezas a camara en {plan.Movimientos.Count} movimientos ({kilosIngresados:0.##} kg)."
            });

            lm.FechaActualizacion = ahora;

            await this.context.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new LiberarJornadaResponse
            {
                MovimientosGenerados = plan.Movimientos.Count,
                PiezasLiberadas = piezas.Count,
                RomaneosLiberados = romaneosLiberados,
                PiezasYaLiberadas = plan.PiezasYaLiberadas,
                KilosIngresados = kilosIngresados
            };
        }

        private static string ArmarMensajeProblemas(LiberacionPlan plan)
        {
            var detalle = string.Join(" ", plan.Problemas
                .Take(MaxProblemasEnMensaje)
                .Select(p => $"En {p.Ubicacion}: {p.Motivo}"));

            var restantes = plan.Problemas.Count - MaxProblemasEnMensaje;
            if (restantes > 0)
                detalle += $" Y {restantes} problema(s) mas.";

            return $"No se libero nada: {plan.Problemas.Count} de {plan.PiezasAProcesar} pieza(s) no se pueden liberar. {detalle}";
        }
    }
}
