using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Meat.Repositories.Migrations
{
    public partial class _11_MultiEspecieEstablecimiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear tabla EstablecimientosEspecies
            migrationBuilder.CreateTable(
                name: "EstablecimientosEspecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EstablecimientoId = table.Column<Guid>(nullable: false),
                    EspecieId = table.Column<string>(nullable: true),
                    FechaActualizacion = table.Column<DateTime>(nullable: false),
                    FechaBaja = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablecimientosEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId",
                table: "EstablecimientosEspecies",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EspecieId",
                table: "EstablecimientosEspecies",
                column: "EspecieId");

            // 2. Migrar datos existentes: crear un registro por cada establecimiento que tenia EspecieId
            migrationBuilder.Sql(@"
                INSERT INTO EstablecimientosEspecies (Id, EstablecimientoId, EspecieId, FechaActualizacion)
                SELECT NEWID(), Id, EspecieId, GETDATE()
                FROM Establecimientos
                WHERE EspecieId IS NOT NULL AND FechaBaja IS NULL
            ");

            // 3. Eliminar columna EspecieId de Establecimientos
            migrationBuilder.DropColumn(
                name: "EspecieId",
                table: "Establecimientos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restaurar columna EspecieId
            migrationBuilder.AddColumn<string>(
                name: "EspecieId",
                table: "Establecimientos",
                nullable: true);

            // Restaurar datos (toma la primera especie si habia multiples)
            migrationBuilder.Sql(@"
                UPDATE e
                SET e.EspecieId = ee.EspecieId
                FROM Establecimientos e
                INNER JOIN (
                    SELECT EstablecimientoId, MIN(EspecieId) AS EspecieId
                    FROM EstablecimientosEspecies
                    WHERE FechaBaja IS NULL
                    GROUP BY EstablecimientoId
                ) ee ON ee.EstablecimientoId = e.Id
            ");

            migrationBuilder.DropTable(name: "EstablecimientosEspecies");
        }
    }
}
