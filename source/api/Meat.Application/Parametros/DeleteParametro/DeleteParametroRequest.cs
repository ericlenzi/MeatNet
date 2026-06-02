using MediatR;

namespace Meat.Application.Parametros.DeleteParametro
{
    public class DeleteParametroRequest : IRequest<DeleteParametroResponse>
    {
        public string Codigo { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
