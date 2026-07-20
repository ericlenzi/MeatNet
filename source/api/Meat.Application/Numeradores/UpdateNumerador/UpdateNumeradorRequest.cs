using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Numeradores.UpdateNumerador
{
    public class UpdateNumeradorRequest : IRequest<UpdateNumeradorResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public string Descripcion { get; set; }
        public string TipoNumerador { get; set; }
        public int UltimoNumero { get; set; }
        public bool Activo { get; set; }
    }
}
