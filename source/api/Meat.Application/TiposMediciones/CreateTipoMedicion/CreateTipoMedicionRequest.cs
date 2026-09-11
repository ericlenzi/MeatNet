using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposMediciones.CreateTipoMedicion
{
    public class CreateTipoMedicionRequest : IRequest<CreateTipoMedicionResponse>
    {
        [Required]
        public string Codigo { get; set; }
        [Required]
        public string Nombre { get; set; }
    }
}
