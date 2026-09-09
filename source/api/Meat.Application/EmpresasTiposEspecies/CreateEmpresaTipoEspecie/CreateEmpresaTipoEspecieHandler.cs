using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Domain.EmpresasTiposEspecies;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.EmpresasTiposEspecies.CreateEmpresaTipoEspecie
{
    /// <summary>
    /// Da de alta una categoria del catalogo global en la configuracion de la empresa activa.
    /// Es el unico lugar donde se copia el peso teorico de referencia, asi vale igual desde la
    /// pantalla y desde el seeder de empresas nuevas.
    /// </summary>
    public class CreateEmpresaTipoEspecieHandler : IRequestHandler<CreateEmpresaTipoEspecieRequest, CreateEmpresaTipoEspecieResponse>
    {
        private readonly MeatContext context;

        public CreateEmpresaTipoEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateEmpresaTipoEspecieResponse> Handle(CreateEmpresaTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var tipoEspecie = await this.context.TiposEspecies
                .FirstOrDefaultAsync(x => x.Codigo == request.TipoEspecieId, cancellationToken);

            if (tipoEspecie == null)
                throw new ValidationException("El tipo de especie no existe.");

            if (!tipoEspecie.Activo)
                throw new ValidationException("El tipo de especie esta dado de baja en el catalogo.");

            var yaConfigurado = await this.context.EmpresasTiposEspecies
                .AnyAsync(x => x.TipoEspecieId == request.TipoEspecieId, cancellationToken);

            if (yaConfigurado)
                throw new ValidationException("La empresa ya tiene configurado ese tipo de especie.");

            var entity = EmpresaTipoEspecieFactory.Create();
            entity.TipoEspecieId = tipoEspecie.Codigo;
            entity.PesoTeorico = request.PesoTeorico ?? tipoEspecie.PesoTeoricoReferencia;
            entity.ERP_Codigo = request.ERP_Codigo;

            this.context.EmpresasTiposEspecies.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateEmpresaTipoEspecieResponse { Id = entity.Id };
        }
    }
}
