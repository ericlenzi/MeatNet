using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _70_DecomisoTotalYParcial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoDecomisoId",
                table: "RomaneosPiezas",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PesoDecomisado",
                table: "RomaneosPiezas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "DecomisoTotal",
                table: "Romaneos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MotivoDecomisoId",
                table: "Romaneos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "MotivosDecomisos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_MotivoDecomisoId",
                table: "RomaneosPiezas",
                column: "MotivoDecomisoId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_MotivoDecomisoId",
                table: "Romaneos",
                column: "MotivoDecomisoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_MotivosDecomisos_MotivoDecomisoId",
                table: "Romaneos",
                column: "MotivoDecomisoId",
                principalTable: "MotivosDecomisos",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezas_MotivosDecomisos_MotivoDecomisoId",
                table: "RomaneosPiezas",
                column: "MotivoDecomisoId",
                principalTable: "MotivosDecomisos",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_MotivosDecomisos_MotivoDecomisoId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_RomaneosPiezas_MotivosDecomisos_MotivoDecomisoId",
                table: "RomaneosPiezas");

            migrationBuilder.DropIndex(
                name: "IX_RomaneosPiezas_MotivoDecomisoId",
                table: "RomaneosPiezas");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_MotivoDecomisoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "MotivoDecomisoId",
                table: "RomaneosPiezas");

            migrationBuilder.DropColumn(
                name: "PesoDecomisado",
                table: "RomaneosPiezas");

            migrationBuilder.DropColumn(
                name: "DecomisoTotal",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "MotivoDecomisoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "MotivosDecomisos");
        }
    }
}
