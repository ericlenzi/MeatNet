using MediatR;
using Meat.Application.IngresosHaciendas; // FamiliaAlmacen
using Meat.Application.Romaneos;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EvaluacionFaena.ActualizarPieza
{
    /// <summary>
    /// Corrige una pieza romaneada antes de liberar (Ciclo I paso 4): peso, tipificacion y camara
    /// destino. Es la ventana de correccion que da la Evaluacion de Faena; una vez liberada, la
    /// pieza es inmutable (R-L3).
    ///
    /// Mantiene coherente lo que el paso 3 dejo montado: la medicion PESO (de la que Peso es
    /// cache), la marca de fuera de rango y los Puntos de las tipificaciones involucradas.
    /// </summary>
    public class ActualizarPiezaHandler : IRequestHandler<ActualizarPiezaRequest, ActualizarPiezaResponse>
    {
        private readonly MeatContext context;

        public ActualizarPiezaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<ActualizarPiezaResponse> Handle(ActualizarPiezaRequest request, CancellationToken cancellationToken)
        {
            var pieza = await this.context.RomaneosPiezas
                .Include(p => p.Romaneo)
                .Include(p => p.Mediciones)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (pieza == null)
                throw new ValidationException("La pieza no existe.");

            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .FirstOrDefaultAsync(x => x.Id == pieza.Romaneo.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La pieza no pertenece a la empresa.");

            // R-L3: liberada es definitiva. Corregirla exigiria un contramovimiento (O-4).
            if (pieza.Liberado || pieza.Romaneo.Liberado)
                throw new ValidationException("La pieza ya fue liberada y no admite cambios.");

            if (pieza.Romaneo.Anulado)
                throw new ValidationException("El romaneo esta anulado y no admite cambios.");

            if (request.Peso <= 0)
                throw new ValidationException("El peso debe ser mayor a cero.");

            if (!request.TipificacionId.HasValue)
                throw new ValidationException("La pieza debe tener una tipificacion.");

            var tipificacion = await this.context.Tipificaciones
                .FirstOrDefaultAsync(t => t.Id == request.TipificacionId.Value
                   
                    && t.Activo, cancellationToken);
            if (tipificacion == null)
                throw new ValidationException("La tipificacion no existe, no esta activa o no pertenece a la empresa.");

            if (tipificacion.EspecieId != pieza.Romaneo.EspecieId)
                throw new ValidationException("La tipificacion no corresponde a la especie de la jornada.");

            // R-L2: el destino tiene que seguir siendo una camara activa de este establecimiento.
            var camaraValida = await (
                from a in this.context.Almacenes
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where a.Id == request.AlmacenDestinoId
                    && a.EstablecimientoId == lm.EstablecimientoId
                    && ta.Familia == FamiliaAlmacen.Camara
                    && a.Activo
                select a.Id).AnyAsync(cancellationToken);
            if (!camaraValida)
                throw new ValidationException("La camara de destino no es una camara activa de este establecimiento.");

            var fueraRango = request.Peso < tipificacion.PesoDesde || request.Peso > tipificacion.PesoHasta;
            if (fueraRango && !request.ForzarFueraRango)
                throw new ValidationException(
                    $"El peso {request.Peso} kg esta fuera del rango {tipificacion.PesoDesde}-{tipificacion.PesoHasta} kg de la tipificacion '{tipificacion.Descripcion}'. Confirme para forzarlo.");

            // Puntos: la pieza deja de contar para la tipificacion anterior y pasa a contar para
            // la nueva, igual que los suma el Tipificador al crear el romaneo.
            if (pieza.TipificacionId != tipificacion.Id)
            {
                var anterior = await this.context.Tipificaciones
                    .FirstOrDefaultAsync(t => t.Id == pieza.TipificacionId, cancellationToken);
                if (anterior != null)
                {
                    anterior.Puntos = Math.Max(0, anterior.Puntos - 1);
                    anterior.FechaActualizacion = DateTime.Now;
                }

                tipificacion.Puntos += 1;
                tipificacion.FechaActualizacion = DateTime.Now;
            }

            pieza.Peso = request.Peso;
            pieza.PesoFueraRango = fueraRango;
            pieza.TipificacionId = tipificacion.Id;
            pieza.AlmacenDestinoId = request.AlmacenDestinoId;

            // Peso es cache de la medicion PESO: si no se actualiza, la medicion queda mintiendo.
            var medicionPeso = pieza.Mediciones
                .FirstOrDefault(m => m.TipoMedicionId == RomaneoConstantes.MedicionPeso);
            if (medicionPeso != null)
                medicionPeso.Valor = request.Peso;

            await this.context.SaveChangesAsync(cancellationToken);

            return new ActualizarPiezaResponse
            {
                Id = pieza.Id,
                Peso = pieza.Peso,
                PesoFueraRango = pieza.PesoFueraRango,
                TipificacionId = pieza.TipificacionId,
                AlmacenDestinoId = pieza.AlmacenDestinoId
            };
        }
    }
}
