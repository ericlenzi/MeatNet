using MediatR;
using Meat.Application.IngresosHaciendas; // FamiliaAlmacen
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Romaneos.GetRenglonesEjecucion
{
    /// <summary>
    /// Renglones de la LM con Cantidad/Faenada/pendiente, el renglon sugerido (menor secuencia
    /// con pendiente) y el proximo garron sugerido. Alimenta el modo hibrido del Tipificador.
    /// </summary>
    public class GetRenglonesEjecucionHandler : IRequestHandler<GetRenglonesEjecucionRequest, GetRenglonesEjecucionResponse>
    {
        private readonly MeatContext context;

        public GetRenglonesEjecucionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRenglonesEjecucionResponse> Handle(GetRenglonesEjecucionRequest request, CancellationToken cancellationToken)
        {
            var lm = await this.context.ListasMatanzas
                .Include(x => x.Establecimiento)
                .Include(x => x.Especie)
                .Include(x => x.Puesto)
                .FirstOrDefaultAsync(x => x.Id == request.ListaMatanzaId, cancellationToken);
            if (lm == null)
                throw new ValidationException("La lista de matanza no existe.");

            var renglones = await (
                from d in this.context.ListasMatanzasDetalles
                join t in this.context.Tropas on d.TropaId equals t.Id
                join a in this.context.Almacenes on d.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on d.TipoEspecieId equals te.Codigo
                join adj in this.context.Almacenes on d.AlmacenDestinoId equals adj.Id into ad
                from destino in ad.DefaultIfEmpty()
                where d.ListaMatanzaId == lm.Id
                orderby d.Secuencia
                select new RenglonEjecucionItem
                {
                    RenglonId = d.Id,
                    TropaId = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    AlmacenId = a.Id,
                    AlmacenNombre = a.Nombre,
                    AlmacenDestinoId = d.AlmacenDestinoId,
                    AlmacenDestinoNombre = destino != null ? destino.Nombre : null,
                    TipoEspecieId = te.Codigo,
                    TipoEspecieNombre = te.Nombre,
                    Secuencia = d.Secuencia,
                    Cantidad = d.Cantidad,
                    CantidadFaenada = d.CantidadFaenada,
                    Pendiente = d.Cantidad - d.CantidadFaenada
                })
                .ToListAsync(cancellationToken);

            // Camaras activas del establecimiento de la LM: opciones del selector de destino del puesto.
            var camaras = await (
                from a in this.context.Almacenes
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where a.EstablecimientoId == lm.EstablecimientoId
                    && ta.Familia == FamiliaAlmacen.Camara
                    && a.Activo
                orderby a.Nombre
                select new CamaraOption { Id = a.Id, Nombre = a.Nombre })
                .ToListAsync(cancellationToken);

            // Cabecera del puesto: los tipificadores habilitados para esta planta y especie, y
            // los metodos de medicion del catalogo. El Tipificador propone el tipificador marcado
            // por defecto y el metodo que tiene configurado el puesto, y el operario los cambia
            // si ese dia el palco trabaja de otra forma.
            var tipificadores = await this.context.Tipificadores
                .Where(t => t.Activo && t.EstablecimientoId == lm.EstablecimientoId && t.EspecieId == lm.EspecieId)
                .OrderByDescending(t => t.PorDefecto).ThenBy(t => t.Nombre)
                .Select(t => new TipificadorOption
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Matricula = t.Matricula,
                    PorDefecto = t.PorDefecto
                })
                .ToListAsync(cancellationToken);

            var tiposMediciones = await this.context.TiposMediciones
                .Where(t => t.Activo)
                .OrderBy(t => t.Codigo)
                .Select(t => new TipoMedicionOption { Codigo = t.Codigo, Nombre = t.Nombre })
                .ToListAsync(cancellationToken);

            var ultimoGarron = await this.context.Romaneos
                .Where(r => r.ListaMatanzaId == lm.Id && !r.Anulado)
                .Select(r => (int?)r.NumeroGarron)
                .MaxAsync(cancellationToken) ?? 0;

            var sugerido = renglones.FirstOrDefault(r => r.Pendiente > 0);

            return new GetRenglonesEjecucionResponse
            {
                ListaMatanzaId = lm.Id,
                NumeroLista = lm.NumeroLista,
                EspecieId = lm.EspecieId,
                EspecieNombre = lm.Especie != null ? lm.Especie.Nombre : lm.EspecieId,
                EstadoListaMatanzaId = lm.EstadoListaMatanzaId,
                ProximoGarron = ultimoGarron + 1,
                RenglonSugeridoId = sugerido?.RenglonId,
                PuestoId = lm.PuestoId,
                PuestoCodigo = lm.Puesto?.CodigoPuesto,
                PuestoNombre = lm.Puesto?.Nombre,
                TipificadorSugeridoId = tipificadores.FirstOrDefault(t => t.PorDefecto)?.Id
                    ?? tipificadores.FirstOrDefault()?.Id,
                TipoMedicionSugeridoId = lm.Puesto?.TipoMedicionId ?? tiposMediciones.FirstOrDefault()?.Codigo,
                Renglones = renglones,
                Camaras = camaras,
                Tipificadores = tipificadores,
                TiposMediciones = tiposMediciones
            };
        }
    }
}
