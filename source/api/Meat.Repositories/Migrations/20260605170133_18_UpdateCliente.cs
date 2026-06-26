using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _18_UpdateCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoActividad",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "NumeroIngresosBrutos",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "NumeroInscripcionRuca",
                table: "Clientes",
                newName: "Renspa");

            migrationBuilder.AddColumn<bool>(
                name: "EsServicioFaena",
                table: "Clientes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsServicioFaena",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Renspa",
                table: "Clientes",
                newName: "NumeroInscripcionRuca");

            migrationBuilder.AddColumn<string>(
                name: "CodigoActividad",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroIngresosBrutos",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
