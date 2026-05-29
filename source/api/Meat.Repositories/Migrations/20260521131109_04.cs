using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuariosSucursales_Roles_RolId",
                table: "UsuariosSucursales");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosSucursales_RolId",
                table: "UsuariosSucursales");

            migrationBuilder.DropColumn(
                name: "RolId",
                table: "UsuariosSucursales");

            migrationBuilder.AddColumn<bool>(
                name: "esMain",
                table: "UsuariosSucursales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "esMain",
                table: "UsuariosSucursales");

            migrationBuilder.AddColumn<string>(
                name: "RolId",
                table: "UsuariosSucursales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_RolId",
                table: "UsuariosSucursales",
                column: "RolId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosSucursales_Roles_RolId",
                table: "UsuariosSucursales",
                column: "RolId",
                principalTable: "Roles",
                principalColumn: "Codigo");
        }
    }
}
