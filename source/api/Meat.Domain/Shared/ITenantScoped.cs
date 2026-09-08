namespace Meat.Domain.Shared
{
    /// <summary>
    /// Marca una entidad como propia de una Empresa (tenant). El MeatContext le aplica
    /// automaticamente el filtro por empresa activa en las lecturas y le asigna el
    /// EmpresaId en las altas, asi el aislamiento no depende de que cada handler lo escriba.
    ///
    /// Regla estructural del modelo: una entidad es ITenantScoped si y solo si su PK es Guid.
    /// Las tablas comunes a todas las empresas son catalogos (PK string Codigo) y no la implementan.
    /// El MeatContext valida esta invariante al construir el modelo.
    /// </summary>
    public interface ITenantScoped
    {
        string EmpresaId { get; set; }
    }
}
