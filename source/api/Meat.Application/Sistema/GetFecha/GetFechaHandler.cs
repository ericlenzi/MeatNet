using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Sistema.GetFecha
{
    public class GetFechaHandler : IRequestHandler<GetFechaRequest, GetFechaResponse>
    {
        public Task<GetFechaResponse> Handle(GetFechaRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new GetFechaResponse
            {
                FechaSistema = DateTime.Now
            });
        }
    }
}