using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Especies.UpdateEspecie
{
    public class UpdateEspecieHandler : IRequestHandler<UpdateEspecieRequest, UpdateEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateEspecieResponse> Handle(UpdateEspecieRequest request, CancellationToken cancellationToken)
        {
            var especie = await this.context.Especies
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo);
            if (especie == null)
            {
                throw new ValidationException("La especie no existe");
            }

            this.mapper.Map(request, especie);

            await this.context.SaveChangesAsync();

            return new UpdateEspecieResponse();
        }
    }
}
