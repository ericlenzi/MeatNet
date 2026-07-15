using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _36_ListaMatanzaEstadoAnulada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                table: "ListasMatanzas");

            // Rename del estado CANCELADA -> ANULADA (catalogo + datos existentes)
            migrationBuilder.Sql(
                "INSERT INTO TiposEstadosListasMatanzas (Codigo, Nombre, Activo) VALUES ('ANULADA', 'Anulada', 1)");
            migrationBuilder.Sql(
                "UPDATE ListasMatanzas SET EstadoListaMatanzaId = 'ANULADA' WHERE EstadoListaMatanzaId = 'CANCELADA'");
            migrationBuilder.Sql(
                "DELETE FROM TiposEstadosListasMatanzas WHERE Codigo = 'CANCELADA'");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "Fecha", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [EstadoListaMatanzaId] <> 'ANULADA'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                table: "ListasMatanzas");

            // Reversa del rename ANULADA -> CANCELADA
            migrationBuilder.Sql(
                "INSERT INTO TiposEstadosListasMatanzas (Codigo, Nombre, Activo) VALUES ('CANCELADA', 'Cancelada', 1)");
            migrationBuilder.Sql(
                "UPDATE ListasMatanzas SET EstadoListaMatanzaId = 'CANCELADA' WHERE EstadoListaMatanzaId = 'ANULADA'");
            migrationBuilder.Sql(
                "DELETE FROM TiposEstadosListasMatanzas WHERE Codigo = 'ANULADA'");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "Fecha", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [EstadoListaMatanzaId] <> 'CANCELADA'");
        }
    }
}
