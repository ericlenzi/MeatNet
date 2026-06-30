using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ExistenciaHacienda.GetExistenciaHacienda
{
    public class GetExistenciaHaciendaRequest : IRequest<GetExistenciaHaciendaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }
    }
}
