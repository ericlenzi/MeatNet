using System;
using MediatR;
using Meat.Application.Shared;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroRequest : RequestBase, IRequest<UpdateParametroResponse>
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool Activo { get; set; }
    }
}
