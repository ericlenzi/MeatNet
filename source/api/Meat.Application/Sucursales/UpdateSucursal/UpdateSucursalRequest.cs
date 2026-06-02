using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Sucursales.UpdateSucursal
{

    public class UpdateSucursalRequest : IRequest<UpdateSucursalResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public Guid EmpresaId { get; set; }
        public bool Activa { get; set; }
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
