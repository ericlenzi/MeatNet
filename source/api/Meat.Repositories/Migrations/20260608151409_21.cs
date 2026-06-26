using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoAnimalId",
                table: "AlmacenesMateriales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenesMateriales_TipoAnimalId",
                table: "AlmacenesMateriales",
                column: "TipoAnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlmacenesMateriales_TiposAnimales_TipoAnimalId",
                table: "AlmacenesMateriales",
                column: "TipoAnimalId",
                principalTable: "TiposAnimales",
                principalColumn: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlmacenesMateriales_TiposAnimales_TipoAnimalId",
                table: "AlmacenesMateriales");

            migrationBuilder.DropIndex(
                name: "IX_AlmacenesMateriales_TipoAnimalId",
                table: "AlmacenesMateriales");

            migrationBuilder.DropColumn(
                name: "TipoAnimalId",
                table: "AlmacenesMateriales");
        }
    }
}
