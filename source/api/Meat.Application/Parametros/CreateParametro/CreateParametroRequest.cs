using Meat.Application.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroRequest : RequestBase, IRequest<CreateParametroResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Valor { get; set; }
    }
}
