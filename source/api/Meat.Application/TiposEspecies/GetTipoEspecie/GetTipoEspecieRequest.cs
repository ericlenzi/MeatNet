using System;
using MediatR;

namespace Meat.Application.TiposEspecies.GetTipoEspecie
{
    public class GetTipoEspecieRequest : IRequest<GetTipoEspecieResponse>
    {
        public Guid Id { get; set; }
    }
}
