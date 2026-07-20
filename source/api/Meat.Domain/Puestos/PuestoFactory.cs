using System;

namespace Meat.Domain.Puestos
{
    public static class PuestoFactory
    {
        public static Puesto Create()
        {
            return new Puesto()
            {
                Id = Guid.NewGuid()
            };
        }

        public static Puesto Create(Guid establecimientoId, string erp_Codigo)
        {
            return new Puesto()
            {
                Id = Guid.NewGuid(),
                EstablecimientoId = establecimientoId,
                Erp_Codigo = erp_Codigo,
                FechaActualizacion = DateTime.Now
            };
        }
    }
}
