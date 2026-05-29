using MediatR;
using System;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroRequest : IRequest<UpdateParametroResponse>
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
    }
}
