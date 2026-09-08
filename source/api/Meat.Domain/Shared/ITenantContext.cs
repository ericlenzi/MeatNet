namespace Meat.Domain.Shared
{
    /// <summary>
    /// Empresa activa de la request en curso. La implementacion la resuelve del claim del JWT.
    /// Devuelve null cuando no hay usuario autenticado (login, migraciones al arrancar), y en
    /// ese caso el filtro por empresa del MeatContext queda inactivo.
    /// </summary>
    public interface ITenantContext
    {
        string EmpresaId { get; }
    }
}
