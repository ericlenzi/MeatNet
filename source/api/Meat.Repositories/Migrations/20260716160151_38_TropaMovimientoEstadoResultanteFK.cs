using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _38_TropaMovimientoEstadoResultanteFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EstadoResultanteId",
                table: "TropasMovimientos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_EstadoResultanteId",
                table: "TropasMovimientos",
                column: "EstadoResultanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TropasMovimientos_TiposEstadosTropas_EstadoResultanteId",
                table: "TropasMovimientos",
                column: "EstadoResultanteId",
                principalTable: "TiposEstadosTropas",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TropasMovimientos_TiposEstadosTropas_EstadoResultanteId",
                table: "TropasMovimientos");

            migrationBuilder.DropIndex(
                name: "IX_TropasMovimientos_EstadoResultanteId",
                table: "TropasMovimientos");

            migrationBuilder.AlterColumn<string>(
                name: "EstadoResultanteId",
                table: "TropasMovimientos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
