using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Establecimientos_Especies_EspecieId",
                table: "Establecimientos");

            migrationBuilder.DropIndex(
                name: "IX_Establecimientos_EspecieId",
                table: "Establecimientos");

            migrationBuilder.DropColumn(
                name: "EspecieId",
                table: "Establecimientos");

            migrationBuilder.RenameColumn(
                name: "esMain",
                table: "UsuariosSucursales",
                newName: "EsMain");

            migrationBuilder.CreateTable(
                name: "EstablecimientosEspecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablecimientosEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EspecieId",
                table: "EstablecimientosEspecies",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId",
                table: "EstablecimientosEspecies",
                column: "EstablecimientoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EstablecimientosEspecies");

            migrationBuilder.RenameColumn(
                name: "EsMain",
                table: "UsuariosSucursales",
                newName: "esMain");

            migrationBuilder.AddColumn<string>(
                name: "EspecieId",
                table: "Establecimientos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_EspecieId",
                table: "Establecimientos",
                column: "EspecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Establecimientos_Especies_EspecieId",
                table: "Establecimientos",
                column: "EspecieId",
                principalTable: "Especies",
                principalColumn: "Codigo");
        }
    }
}
