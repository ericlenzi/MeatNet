using System;
using MediatR;

namespace Meat.Application.TiposEspecies.DeleteTipoEspecie
{
    public class DeleteTipoEspecieRequest : IRequest<DeleteTipoEspecieResponse>
    {
        public Guid Id { get; set; }
    }
}
