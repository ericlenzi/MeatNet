using System;

namespace Meat.Domain.Empresas
{
    public static class EmpresaFactory
    {
        public static Empresa Create()
        {
            return new Empresa()
            {
                Id = Guid.NewGuid(),
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
