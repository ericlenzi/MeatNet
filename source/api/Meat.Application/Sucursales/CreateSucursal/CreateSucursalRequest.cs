using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Sucursales.CreateSucursal
{
    public class CreateSucursalRequest : IRequest<CreateSucursalResponse>
    {
        [Required]
        public string NumeroSucursal { get; set; }
        
        [Required]
        public string Nombre { get; set; }
        
        public string Direccion { get; set; }
    }
}
