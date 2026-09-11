using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _78_MermaOreo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MermaOreo",
                table: "EstablecimientosEspecies",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MermaOreoReferencia",
                table: "Especies",
                type: "float",
                nullable: true);

            // Merma de oreo de referencia del rubro: lo que la media res pierde en la camara
            // entre la pesada caliente y la fria. El 2% es el valor que cita la bibliografia y
            // el que ya estaba escrito en AnalisisFaena.md; cada planta lo ajusta en su
            // establecimiento cuando mide el suyo.
            migrationBuilder.Sql(@"
                UPDATE Especies SET MermaOreoReferencia = 2
                WHERE Codigo IN ('V', 'P') AND MermaOreoReferencia IS NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MermaOreo",
                table: "EstablecimientosEspecies");

            migrationBuilder.DropColumn(
                name: "MermaOreoReferencia",
                table: "Especies");
        }
    }
}
