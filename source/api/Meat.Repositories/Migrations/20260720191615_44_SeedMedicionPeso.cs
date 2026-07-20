using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _44_SeedMedicionPeso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // PESO es una medicion requerida por la Ejecucion de Faena (cada pieza del romaneo
            // graba su medicion PESO). Se siembra por sistema, no depende del script del usuario.
            migrationBuilder.InsertData(
                table: "TiposMediciones",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "PESO", "Peso", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposMediciones",
                keyColumn: "Codigo",
                keyValue: "PESO");
        }
    }
}
