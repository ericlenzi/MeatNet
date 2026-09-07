using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.DespiecesMateriales.GetDespiecesMateriales
{
    public class GetDespiecesMaterialesResponse : ResponseListBase<IEnumerable<DespieceMaterialItem>>
    {
    }

    public class DespieceMaterialItem
    {
        public Guid Id { get; set; }
        public Guid MaterialOrigenId { get; set; }
        public string MaterialOrigenCodigo { get; set; }
        public string MaterialOrigenNombre { get; set; }
        public Guid MaterialDestinoId { get; set; }
        public string MaterialDestinoCodigo { get; set; }
        public string MaterialDestinoNombre { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
        public bool Activo { get; set; }
    }
}
