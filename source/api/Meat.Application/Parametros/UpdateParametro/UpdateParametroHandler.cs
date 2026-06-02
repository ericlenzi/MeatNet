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
            var parametro = await this.context.Parametros
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);
            if (parametro == null)
            {
                throw new ValidationException("El parametro no existe");
            }

            this.mapper.Map(request, parametro);

            await this.context.SaveChangesAsync();

            return new UpdateParametroResponse();
        }
    }
}
