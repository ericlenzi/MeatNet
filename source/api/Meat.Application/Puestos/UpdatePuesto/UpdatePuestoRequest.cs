using MediatR;
using System;

namespace Meat.Application.Puestos.UpdatePuesto
{

    public class UpdatePuestoRequest : IRequest<UpdatePuestoResponse>
    {
        public Guid Id { get; set; }

        public string NumeroPuesto { get; set; }

        public Guid EstablecimientoId { get; set; }

        public string Erp_Codigo { get; set; }
    }
}
