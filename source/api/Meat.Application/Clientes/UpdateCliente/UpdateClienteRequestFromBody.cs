using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Clientes.UpdateCliente
{
    public class UpdateClienteRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string TipoClienteId { get; set; }
        public string NumeroCuit { get; set; }
        public string NumeroIngresosBrutos { get; set; }
        public string NumeroInscripcionRuca { get; set; }
        public string CodigoActividad { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
