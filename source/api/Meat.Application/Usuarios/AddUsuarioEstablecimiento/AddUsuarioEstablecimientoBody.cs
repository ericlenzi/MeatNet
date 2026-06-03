using System;

namespace Meat.Application.Usuarios.AddUsuarioEstablecimiento
{
    public class AddUsuarioEstablecimientoBody
    {
        public Guid EstablecimientoId { get; set; }
        public bool EsMain { get; set; }
    }
}
