using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _27_AddUniqueIndexNumeradoresTropas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId",
                table: "NumeradoresTropas");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId_EspecieCodigo",
                table: "NumeradoresTropas",
                columns: new[] { "ClienteEstablecimientoId", "EspecieCodigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId_EspecieCodigo",
                table: "NumeradoresTropas");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId",
                table: "NumeradoresTropas",
                column: "ClienteEstablecimientoId");
        }
    }
}
