using MediatR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meat.Application.Sucursales.CreateSucursal
{
    public class CreateSucursalRequest : IRequest<CreateSucursalResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [Required]
        public string CodigoSucursal { get; set; }

        [Required]
        public string Nombre { get; set; }

        public Guid EmpresaId { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Erp_Codigo { get; set; }
        public string Color { get; set; }
    }
}
