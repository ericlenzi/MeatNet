using MediatR;
using Meat.Application.EvaluacionFaena.Shared;
using Meat.Application.ListasMatanzas;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EvaluacionFaena.PrevisualizarLiberacion
{
    /// <summary>
    /// Muestra que pasaria al liberar la jornada, sin escribir nada: cuanto entraria a cada
    /// camara y que piezas lo impiden. Como la liberacion es todo o nada, esta es la pantalla
    /// donde el usuario ve y corrige los problemas antes de intentarla.
    ///
    /// A diferencia de liberar, no exige que la lista este Finalizada: sirve para anticipar
    /// problemas durante la jornada; el response dice si ya se puede liberar.
    /// </summary>
    public class PrevisualizarLiberacionHandler : IRequestHandler<PrevisualizarLiberacionRequest, PrevisualizarLiberacionResponse>
    {
        private readonly MeatContext context;

        public PrevisualizarLiberacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<PrevisualizarLiberacionResponse> Handle(PrevisualizarLiberacionRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            var plan = await LiberacionCalculo.CalcularAsync(
                this.context, lm.Id, lm.EstablecimientoId, lm.NumeroLista.ToString(), cancellationToken);

            var finalizada = lm.EstadoListaMatanzaId == EstadosListaMatanza.Finalizada;

            var response = new PrevisualizarLiberacionResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                JornadaFinalizada = finalizada,
                PiezasAProcesar = plan.PiezasAProcesar,
                PiezasYaLiberadas = plan.PiezasYaLiberadas,
                // Neto: en un cuarteo el ingreso de la media res y su baja se cancelan, y quedan
                // los kilos de los cuartos. Sumar solo las altas contaria la carne dos veces.
                KilosAIngresar = plan.Movimientos.Sum(m => m.Peso),
                Problemas = plan.Problemas.Select(p => new ProblemaItem
                {
                    PiezaId = p.PiezaId,
                    NumeroRomaneo = p.NumeroRomaneo,
                    NumeroGarron = p.NumeroGarron,
                    Letra = p.Letra,
                    Motivo = p.Motivo
                }).ToList()
            };

            // Neto por (camara, material): lo que efectivamente quedaria en existencia. Una media
            // res cuarteada se cancela contra su baja y no aparece; aparecen sus cuartos.
            response.Resumen = plan.Movimientos
                .GroupBy(m => new { m.AlmacenId, m.AlmacenNombre, m.MaterialId, m.MaterialCodigo, m.MaterialNombre })
                .Select(g => new ResumenExistenciaItem
                {
                    AlmacenId = g.Key.AlmacenId,
                    AlmacenNombre = g.Key.AlmacenNombre,
                    MaterialId = g.Key.MaterialId,
                    MaterialCodigo = g.Key.MaterialCodigo,
                    MaterialNombre = g.Key.MaterialNombre,
                    Cantidad = g.Sum(m => m.Cantidad),
                    Peso = g.Sum(m => m.Peso)
                })
                .Where(r => r.Cantidad != 0)
                .OrderBy(r => r.AlmacenNombre).ThenBy(r => r.MaterialNombre)
                .ToList();

            response.PuedeLiberar = finalizada && plan.PiezasAProcesar > 0 && !plan.TieneProblemas;
            response.MotivoBloqueo = ArmarMotivoBloqueo(plan, finalizada);

            return response;
        }

        private static string ArmarMotivoBloqueo(LiberacionPlan plan, bool finalizada)
        {
            if (!finalizada)
                return "La jornada todavia no esta finalizada. Cierre la lista para poder liberar.";

            if (plan.PiezasAProcesar == 0)
                return plan.PiezasYaLiberadas > 0
                    ? "La jornada ya fue liberada."
                    : "La jornada no tiene romaneos para liberar.";

            if (plan.TieneProblemas)
                return $"{plan.Problemas.Count} de {plan.PiezasAProcesar} pieza(s) no se pueden liberar. Corrijalas para habilitar la liberacion.";

            return null;
        }
    }
}
