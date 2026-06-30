using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _32_SeedCorralesCaidosMuertos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposAlmacenes",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "CORRAL_CAIDOS", "Corral de Caidos", true },
                    { "CORRAL_MUERTOS", "Corral de Muertos", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CORRAL_CAIDOS");
            migrationBuilder.DeleteData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CORRAL_MUERTOS");
        }
    }
}
