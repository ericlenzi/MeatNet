using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _55_UnidadFaenaTipoMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoMaterialId",
                table: "UnidadesFaenas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_TipoMaterialId",
                table: "UnidadesFaenas",
                column: "TipoMaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesFaenas_TiposMateriales_TipoMaterialId",
                table: "UnidadesFaenas",
                column: "TipoMaterialId",
                principalTable: "TiposMateriales",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnidadesFaenas_TiposMateriales_TipoMaterialId",
                table: "UnidadesFaenas");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesFaenas_TipoMaterialId",
                table: "UnidadesFaenas");

            migrationBuilder.DropColumn(
                name: "TipoMaterialId",
                table: "UnidadesFaenas");
        }
    }
}
