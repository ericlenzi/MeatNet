using System;
using System.Collections.Generic;
using MediatR;

namespace Meat.Application.Puestos.GetPuesto
{

    public class GetPuestoRequest : IRequest<GetPuestoResponse>
    {
        public Guid Id { get; set; }
    }

}