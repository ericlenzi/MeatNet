using MediatR;
using Meat.Application.Shared;
using Meat.Domain.Numeradores;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores.CreateNumerador
{
    public class CreateNumeradorHandler : IRequestHandler<CreateNumeradorRequest, CreateNumeradorResponse>
    {
        private readonly MeatContext context;

        public CreateNumeradorHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateNumeradorResponse> Handle(CreateNumeradorRequest request, CancellationToken cancellationToken)
        {
            var establecimiento = await this.context.Establecimientos
                .Include(e => e.Empresa)
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId
                    && e.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (establecimiento == null)
                throw new ValidationException("El establecimiento activo no es valido.");

            var especieExiste = await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieCodigo, cancellationToken);
            if (!especieExiste)
                throw new ValidationException("La especie indicada no existe.");

            var existe = await this.context.Numeradores
                .AnyAsync(n => n.EstablecimientoId == request.EstablecimientoId
                    && n.EspecieCodigo == request.EspecieCodigo
                    && n.Codigo == request.Codigo, cancellationToken);
            if (existe)
                throw new ValidationException("Ya existe un numerador con ese codigo para el establecimiento y especie.");

            var entity = NumeradorFactory.Create();
            entity.EstablecimientoId = request.EstablecimientoId;
            entity.EspecieCodigo = request.EspecieCodigo;
            entity.Codigo = request.Codigo;
            entity.Descripcion = request.Descripcion;
            entity.TipoNumerador = request.TipoNumerador;
            entity.UltimoNumero = request.UltimoNumero;
            entity.Activo = true;

            this.context.Numeradores.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateNumeradorResponse { Id = entity.Id };
        }
    }
}
