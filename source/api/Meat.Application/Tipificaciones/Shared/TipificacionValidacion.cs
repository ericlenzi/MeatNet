using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.Shared
{
    /// <summary>Validaciones comunes de FKs y rango de peso para Tipificaciones.</summary>
    public static class TipificacionValidacion
    {
        public static async Task ValidateAsync(
            MeatContext context,
            string especieId,
            string tipoEspecieId,
            string unidadFaenaId,
            string destinoComercialId,
            string tipificacionOficialId,
            string unidadMedidaId,
            double pesoDesde,
            double pesoHasta,
            CancellationToken cancellationToken)
        {
            if (!await context.Especies.AnyAsync(e => e.Codigo == especieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            if (!string.IsNullOrEmpty(tipoEspecieId)
                && !await context.TiposEspecies.AnyAsync(t => t.Id == tipoEspecieId && t.EspecieId == especieId, cancellationToken))
                throw new ValidationException("La categoria (tipo de especie) no es valida para la especie.");

            if (string.IsNullOrEmpty(unidadFaenaId)
                || !await context.UnidadesFaenas.AnyAsync(u => u.Codigo == unidadFaenaId, cancellationToken))
                throw new ValidationException("La unidad de faena indicada no existe.");

            if (!string.IsNullOrEmpty(destinoComercialId)
                && !await context.DestinosComerciales.AnyAsync(d => d.Codigo == destinoComercialId, cancellationToken))
                throw new ValidationException("El destino comercial indicado no existe.");

            if (!string.IsNullOrEmpty(tipificacionOficialId)
                && !await context.TipificacionesOficiales.AnyAsync(t => t.Codigo == tipificacionOficialId, cancellationToken))
                throw new ValidationException("La tipificacion oficial indicada no existe.");

            if (!string.IsNullOrEmpty(unidadMedidaId)
                && !await context.UnidadesMedidas.AnyAsync(u => u.Codigo == unidadMedidaId, cancellationToken))
                throw new ValidationException("La unidad de medida indicada no existe.");

            if (pesoHasta > 0 && pesoDesde > pesoHasta)
                throw new ValidationException("El peso desde no puede ser mayor al peso hasta.");
        }
    }
}
