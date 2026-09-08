using Meat.Application.Shared;
using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Numeradores.CreateNumerador
{
    public class CreateNumeradorRequest : RequestBase, IRequest<CreateNumeradorResponse>
    {

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
