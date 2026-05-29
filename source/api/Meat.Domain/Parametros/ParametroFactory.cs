namespace Meat.Domain.Parametros
{
    public static class ParametroFactory
    {
        public static Parametro Create(string codigo, string valor)
        {
            return new Parametro()
            {
                Id = System.Guid.NewGuid(),
                Codigo = codigo,
                Valor = valor,
                FechaActualizacion = System.DateTime.Now
            };
        }
    }
}
