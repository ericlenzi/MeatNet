using MediatR;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroRequest : IRequest<UpdateParametroResponse>
    {
        public string Codigo { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string CodigoEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool Activo { get; set; }
    }
}
