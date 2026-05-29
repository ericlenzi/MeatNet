using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroHandler : IRequestHandler<CreateParametroRequest, CreateParametroResponse>
    {
        private readonly MeatContext context;

        public CreateParametroHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateParametroResponse> Handle(CreateParametroRequest request, CancellationToken cancellationToken)
        {
            if (this.context.Parametros.FirstOrDefault(x => x.Codigo == request.Codigo) != null)
            {
                throw new ValidationException("Ya existe un parámetro para el código seleccionado");
            }

            var parametro = Domain.Parametros.ParametroFactory.Create(request.Codigo, request.Valor);

            this.context.Parametros.Add(parametro);

            #region ParametrosSucursales

            var sucursales = context.Sucursales.ToList();
            foreach (var item in sucursales)
            {
                var parametroSucursal = Domain.ParametrosSucursales.ParametroSucursalFactory.Create(item.Id, parametro.Id, request.Valor);
                this.context.ParametrosSucursales.Add(parametroSucursal);
            }

            #endregion ParametrosSucursales

            await this.context.SaveChangesAsync();

            return new CreateParametroResponse()
            {
                Id = parametro.Id,
            };
        }
    }
}
