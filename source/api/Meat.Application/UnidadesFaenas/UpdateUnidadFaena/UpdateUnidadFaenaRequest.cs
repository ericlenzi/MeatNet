using MediatR;
using System.Text.Json.Serialization;

namespace Meat.Application.UnidadesFaenas.UpdateUnidadFaena
{
    public class UpdateUnidadFaenaRequest : IRequest<UpdateUnidadFaenaResponse>
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        public string EspecieId { get; set; }
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
