using System;

namespace Meat.Domain.MovimientosCamaras
{
    public static class MovimientoCamaraFactory
    {
        public static MovimientoCamara Create()
        {
            return new MovimientoCamara
            {
                Id = Guid.NewGuid(),
                Fecha = DateTime.Now
            };
        }
    }
}
