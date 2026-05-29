using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroHandler : IRequestHandler<UpdateParametroRequest, UpdateParametroResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateParametroHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateParametroResponse> Handle(UpdateParametroRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.Parametros.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (parametro == null)
            {
                throw new ValidationException("El parámetro no existe");
            }

            if (await this.context.Parametros.FirstOrDefaultAsync(x => x.Id != request.Id && x.Codigo == request.Codigo) != null)
            {
                throw new ValidationException("Ya existe un parámetro para el código seleccionado");
            }

            this.mapper.Map(request, parametro);

            parametro.FechaActualizacion = System.DateTime.Now;

            #region ParametrosSucursales

            var parametrosSucursales = context.ParametrosSucursales.Where(x => x.ParametroId == request.Id);
            foreach (var item in parametrosSucursales)
            {
                item.Valor = request.Valor;
                item.FechaActualizacion = System.DateTime.Now;
                context.ParametrosSucursales.Update(item);
            }

            #endregion ParametrosSucursales

            await this.context.SaveChangesAsync();

            return new UpdateParametroResponse();
        }
    }
}
