using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _68_DenticionYContusionEnElPalco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Denticiones_TiposDenticiones_TipoDenticionId",
                table: "Denticiones");

            migrationBuilder.DropTable(
                name: "TiposDenticiones");

            migrationBuilder.DropIndex(
                name: "IX_Denticiones_TipoDenticionId",
                table: "Denticiones");

            migrationBuilder.DropColumn(
                name: "TipoDenticionId",
                table: "Denticiones");

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "TiposContusiones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TipoContusionId",
                table: "RomaneosPiezas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DenticionId",
                table: "Romaneos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "Denticiones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_TipoContusionId",
                table: "RomaneosPiezas",
                column: "TipoContusionId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_DenticionId",
                table: "Romaneos",
                column: "DenticionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Denticiones_DenticionId",
                table: "Romaneos",
                column: "DenticionId",
                principalTable: "Denticiones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezas_TiposContusiones_TipoContusionId",
                table: "RomaneosPiezas",
                column: "TipoContusionId",
                principalTable: "TiposContusiones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Denticiones_DenticionId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_RomaneosPiezas_TiposContusiones_TipoContusionId",
                table: "RomaneosPiezas");

            migrationBuilder.DropIndex(
                name: "IX_RomaneosPiezas_TipoContusionId",
                table: "RomaneosPiezas");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_DenticionId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "TiposContusiones");

            migrationBuilder.DropColumn(
                name: "TipoContusionId",
                table: "RomaneosPiezas");

            migrationBuilder.DropColumn(
                name: "DenticionId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "Denticiones");

            migrationBuilder.AddColumn<string>(
                name: "TipoDenticionId",
                table: "Denticiones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposDenticiones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDenticiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TiposDenticiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Denticiones_TipoDenticionId",
                table: "Denticiones",
                column: "TipoDenticionId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposDenticiones_EspecieId",
                table: "TiposDenticiones",
                column: "EspecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Denticiones_TiposDenticiones_TipoDenticionId",
                table: "Denticiones",
                column: "TipoDenticionId",
                principalTable: "TiposDenticiones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
