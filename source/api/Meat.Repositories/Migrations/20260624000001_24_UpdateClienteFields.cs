using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _24_UpdateClienteFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Renspa",
                table: "Clientes",
                newName: "NumeroIngresosBrutos");

            migrationBuilder.DropColumn(
                name: "EsServicioFaena",
                table: "Clientes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumeroIngresosBrutos",
                table: "Clientes",
                newName: "Renspa");

            migrationBuilder.AddColumn<bool>(
                name: "EsServicioFaena",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
