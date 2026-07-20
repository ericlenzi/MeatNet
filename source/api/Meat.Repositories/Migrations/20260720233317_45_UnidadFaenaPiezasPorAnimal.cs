using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _45_UnidadFaenaPiezasPorAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnidadComplementaria",
                table: "UnidadesFaenas",
                newName: "PiezasPorAnimal");

            migrationBuilder.AddColumn<bool>(
                name: "PorDefecto",
                table: "UnidadesFaenas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Poblar PiezasPorAnimal desde CantidadCuartos (una res entera = 4 cuartos):
            // RES(4)->1, MEDIA RES(2)->2, CUARTO(1)->4. El valor viejo (UnidadComplementaria) era 0.
            migrationBuilder.Sql(
                "UPDATE UnidadesFaenas SET PiezasPorAnimal = " +
                "CASE WHEN CantidadCuartos > 0 THEN 4 / CantidadCuartos ELSE 1 END;");

            // Marcar una unidad por defecto por especie (la de menor Numero), antes del indice unico.
            migrationBuilder.Sql(
                "UPDATE UnidadesFaenas SET PorDefecto = 1 WHERE Id IN (" +
                "SELECT Id FROM (SELECT Id, ROW_NUMBER() OVER (PARTITION BY EspecieId ORDER BY Numero) AS rn " +
                "FROM UnidadesFaenas WHERE FechaBaja IS NULL) t WHERE t.rn = 1);");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EspecieId",
                table: "UnidadesFaenas",
                column: "EspecieId",
                unique: true,
                filter: "[FechaBaja] IS NULL AND [PorDefecto] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnidadesFaenas_EspecieId",
                table: "UnidadesFaenas");

            migrationBuilder.DropColumn(
                name: "PorDefecto",
                table: "UnidadesFaenas");

            migrationBuilder.RenameColumn(
                name: "PiezasPorAnimal",
                table: "UnidadesFaenas",
                newName: "UnidadComplementaria");
        }
    }
}
