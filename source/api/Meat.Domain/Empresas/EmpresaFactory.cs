using System;

namespace Meat.Domain.Empresas
{
    public static class EmpresaFactory
    {
        /// <summary>
        /// La PK es el codigo de negocio elegido por el usuario, no se autogenera.
        /// </summary>
        public static Empresa Create(string id)
        {
            return new Empresa()
            {
                Id = id,
                FechaActualizacion = DateTime.Now,
                Activo = true
            };
        }
    }
}
