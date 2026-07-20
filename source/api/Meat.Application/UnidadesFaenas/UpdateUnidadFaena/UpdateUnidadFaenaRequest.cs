using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.UnidadesFaenas.UpdateUnidadFaena
{
    public class UpdateUnidadFaenaRequest : IRequest<UpdateUnidadFaenaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string EspecieId { get; set; }
        public int Numero { get; set; }
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int PiezasPorAnimal { get; set; }
        public bool PorDefecto { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
