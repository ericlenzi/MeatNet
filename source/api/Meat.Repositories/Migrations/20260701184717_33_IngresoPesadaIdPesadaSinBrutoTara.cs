using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _33_IngresoPesadaIdPesadaSinBrutoTara : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PesoBruto",
                table: "IngresosHaciendas");

            migrationBuilder.DropColumn(
                name: "Tara",
                table: "IngresosHaciendas");

            migrationBuilder.AddColumn<string>(
                name: "IdPesada",
                table: "IngresosHaciendasPesadas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPesada",
                table: "IngresosHaciendasPesadas");

            migrationBuilder.AddColumn<double>(
                name: "PesoBruto",
                table: "IngresosHaciendas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Tara",
                table: "IngresosHaciendas",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
