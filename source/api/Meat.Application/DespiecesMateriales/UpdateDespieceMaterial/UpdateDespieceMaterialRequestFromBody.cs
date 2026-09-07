using System;

namespace Meat.Application.DespiecesMateriales.UpdateDespieceMaterial
{
    public class UpdateDespieceMaterialRequestFromBody
    {
        public Guid MaterialOrigenId { get; set; }
        public Guid MaterialDestinoId { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
        public bool Activo { get; set; }
    }
}
