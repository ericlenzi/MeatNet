using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Parametros.CreateParametro
{
    public class CreateParametroRequest : IRequest<CreateParametroResponse>
    {
        [Required]
        public string Codigo { get; set; }

        public string Valor { get; set; }
    }
}
