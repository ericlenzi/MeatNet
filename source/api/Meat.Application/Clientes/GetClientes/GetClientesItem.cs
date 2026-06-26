using System;

namespace Meat.Application.Clientes.GetClientes
{
    public class GetClientesItem
    {
        public Guid Id { get; set; }
        public string CodigoCliente { get; set; }
        public string Nombre { get; set; }
        public string TipoClienteId { get; set; }
        public string TipoClienteNombre { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
