using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _19 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PesoTeorico",
                table: "Materiales",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMaterialId",
                table: "Materiales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnidadMedidaId",
                table: "Materiales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposMateriales",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMateriales", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedidas",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedidas", x => x.Codigo);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_TipoMaterialId",
                table: "Materiales",
                column: "TipoMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_UnidadMedidaId",
                table: "Materiales",
                column: "UnidadMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_TiposMateriales_TipoMaterialId",
                table: "Materiales",
                column: "TipoMaterialId",
                principalTable: "TiposMateriales",
                principalColumn: "Codigo");

            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_UnidadesMedidas_UnidadMedidaId",
                table: "Materiales",
                column: "UnidadMedidaId",
                principalTable: "UnidadesMedidas",
                principalColumn: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_TiposMateriales_TipoMaterialId",
                table: "Materiales");

            migrationBuilder.DropForeignKey(
                name: "FK_Materiales_UnidadesMedidas_UnidadMedidaId",
                table: "Materiales");

            migrationBuilder.DropTable(
                name: "TiposMateriales");

            migrationBuilder.DropTable(
                name: "UnidadesMedidas");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_TipoMaterialId",
                table: "Materiales");

            migrationBuilder.DropIndex(
                name: "IX_Materiales_UnidadMedidaId",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "PesoTeorico",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "TipoMaterialId",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "UnidadMedidaId",
                table: "Materiales");
        }
    }
}
