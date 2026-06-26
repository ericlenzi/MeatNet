using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Clientes.CreateCliente
{
    public class CreateClienteRequest : IRequest<CreateClienteResponse>
    {
        [Required]
        public string CodigoCliente { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string TipoClienteId { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
