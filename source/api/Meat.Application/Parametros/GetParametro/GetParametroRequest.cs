using MediatR;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroRequest : IRequest<GetParametroResponse>
    {
        public string Codigo { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
