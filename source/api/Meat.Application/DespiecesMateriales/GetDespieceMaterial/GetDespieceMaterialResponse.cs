using System;

namespace Meat.Application.DespiecesMateriales.GetDespieceMaterial
{
    public class GetDespieceMaterialResponse
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
