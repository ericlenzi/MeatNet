using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.UnidadesFaenas.CreateUnidadFaena
{
    public class CreateUnidadFaenaRequest : IRequest<CreateUnidadFaenaResponse>
    {
        [Required]
        public string EspecieId { get; set; }
        public int Numero { get; set; }
        [Required]
        public string Nombre { get; set; }
        public int CantidadCuartos { get; set; }
        public int UnidadComplementaria { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
