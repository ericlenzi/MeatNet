using System;
using System.Collections.Generic;

namespace Meat.Application.Usuarios.GetUsuarioEstablecimientos
{
    public class GetUsuarioEstablecimientosResponse
    {
        public IEnumerable<UsuarioEstablecimientoItem> Data { get; set; }
    }

    public class UsuarioEstablecimientoItem
    {
        public Guid Id { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public string CodigoSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public bool EsMain { get; set; }
    }
}
