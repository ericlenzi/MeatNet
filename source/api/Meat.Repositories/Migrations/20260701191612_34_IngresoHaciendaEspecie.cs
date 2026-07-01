using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _34_IngresoHaciendaEspecie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EspecieId",
                table: "IngresosHaciendas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EspecieId",
                table: "IngresosHaciendas",
                column: "EspecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendas_Especies_EspecieId",
                table: "IngresosHaciendas",
                column: "EspecieId",
                principalTable: "Especies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngresosHaciendas_Especies_EspecieId",
                table: "IngresosHaciendas");

            migrationBuilder.DropIndex(
                name: "IX_IngresosHaciendas_EspecieId",
                table: "IngresosHaciendas");

            migrationBuilder.DropColumn(
                name: "EspecieId",
                table: "IngresosHaciendas");
        }
    }
}
