using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.QuitarRenglonListaMatanza
{
    public class QuitarRenglonListaMatanzaRequest : RequestBase, IRequest<QuitarRenglonListaMatanzaResponse>
    {
        public Guid Id { get; set; }                       // lista de matanza
        public Guid RenglonId { get; set; }

    }

    public class QuitarRenglonListaMatanzaResponse
    {
    }
}
