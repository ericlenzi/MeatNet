using System;

namespace Meat.Domain.Clientes
{
    public static class ClienteFactory
    {
        public static Cliente Create()
        {
            return new Cliente()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
