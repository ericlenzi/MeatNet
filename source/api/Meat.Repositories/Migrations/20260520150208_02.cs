using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VersionPos",
                table: "Puestos",
                newName: "Nombre");

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Puestos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Puestos");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Puestos",
                newName: "VersionPos");
        }
    }
}
