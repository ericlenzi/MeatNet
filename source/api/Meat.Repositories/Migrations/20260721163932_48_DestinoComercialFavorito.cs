using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _48_DestinoComercialFavorito : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorito",
                table: "DestinosComerciales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DestinosComerciales_Favorito",
                table: "DestinosComerciales",
                column: "Favorito",
                unique: true,
                filter: "[Favorito] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DestinosComerciales_Favorito",
                table: "DestinosComerciales");

            migrationBuilder.DropColumn(
                name: "Favorito",
                table: "DestinosComerciales");
        }
    }
}
