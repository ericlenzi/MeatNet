using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Numeradores.CreateNumerador
{
    public class CreateNumeradorRequest : IRequest<CreateNumeradorResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid EstablecimientoId { get; set; }
        [Required]
        public string EspecieCodigo { get; set; }
        [Required]
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        [Required]
        public string TipoNumerador { get; set; }
        public int UltimoNumero { get; set; }
    }
}
