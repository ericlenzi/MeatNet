using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _54_MaterialCodigoUnicoYSeedTiposMateriales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CodigoMaterial",
                table: "Materiales",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_CodigoMaterial",
                table: "Materiales",
                column: "CodigoMaterial",
                unique: true,
                filter: "[FechaBaja] IS NULL");

            // Catalogo de tipos de material (clasifica el producto terminado del Ciclo I).
            migrationBuilder.InsertData(
                table: "TiposMateriales",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "MEDIA_RES", "Media Res", true },
                    { "RES", "Res", true },
                    { "CUARTO", "Cuarto", true },
                    { "SUBPRODUCTO", "Subproducto", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "TiposMateriales", keyColumn: "Codigo", keyValue: "MEDIA_RES");
            migrationBuilder.DeleteData(table: "TiposMateriales", keyColumn: "Codigo", keyValue: "RES");
            migrationBuilder.DeleteData(table: "TiposMateriales", keyColumn: "Codigo", keyValue: "CUARTO");
            migrationBuilder.DeleteData(table: "TiposMateriales", keyColumn: "Codigo", keyValue: "SUBPRODUCTO");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_CodigoMaterial",
                table: "Materiales");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoMaterial",
                table: "Materiales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
