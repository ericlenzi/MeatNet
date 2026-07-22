using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _51_RomaneoEstablecimientoNumeroUnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstablecimientoId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill de los romaneos existentes: el establecimiento sale de su lista de matanza.
            // Va antes del indice y la FK, que si no fallarian contra el Guid vacio del default.
            migrationBuilder.Sql(@"
UPDATE r
SET r.EstablecimientoId = l.EstablecimientoId
FROM Romaneos r
INNER JOIN ListasMatanzas l ON l.Id = r.ListaMatanzaId;");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EstablecimientoId_EspecieId_NumeroRomaneo",
                table: "Romaneos",
                columns: new[] { "EstablecimientoId", "EspecieId", "NumeroRomaneo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Establecimientos_EstablecimientoId",
                table: "Romaneos",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Establecimientos_EstablecimientoId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_EstablecimientoId_EspecieId_NumeroRomaneo",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "EstablecimientoId",
                table: "Romaneos");
        }
    }
}
