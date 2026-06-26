using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _28_AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuariosSucursales_UsuarioId",
                table: "UsuariosSucursales");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosEstablecimientos_UsuarioId",
                table: "UsuariosEstablecimientos");

            migrationBuilder.DropIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId",
                table: "EstablecimientosEspecies");

            migrationBuilder.DropIndex(
                name: "IX_ClientesEstablecimientos_ClienteId",
                table: "ClientesEstablecimientos");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoSucursal",
                table: "Sucursales",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoEstablecimiento",
                table: "Establecimientos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoCliente",
                table: "Clientes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_UsuarioId_SucursalId",
                table: "UsuariosSucursales",
                columns: new[] { "UsuarioId", "SucursalId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_UsuarioId_EstablecimientoId",
                table: "UsuariosEstablecimientos",
                columns: new[] { "UsuarioId", "EstablecimientoId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UserName",
                table: "Usuarios",
                column: "UserName",
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_CodigoSucursal",
                table: "Sucursales",
                column: "CodigoSucursal",
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId_EspecieId",
                table: "EstablecimientosEspecies",
                columns: new[] { "EstablecimientoId", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_CodigoEstablecimiento",
                table: "Establecimientos",
                column: "CodigoEstablecimiento",
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_ClienteId_EstablecimientoId",
                table: "ClientesEstablecimientos",
                columns: new[] { "ClienteId", "EstablecimientoId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_CodigoCliente",
                table: "Clientes",
                column: "CodigoCliente",
                unique: true,
                filter: "[FechaBaja] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UsuariosSucursales_UsuarioId_SucursalId",
                table: "UsuariosSucursales");

            migrationBuilder.DropIndex(
                name: "IX_UsuariosEstablecimientos_UsuarioId_EstablecimientoId",
                table: "UsuariosEstablecimientos");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_UserName",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Sucursales_CodigoSucursal",
                table: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId_EspecieId",
                table: "EstablecimientosEspecies");

            migrationBuilder.DropIndex(
                name: "IX_Establecimientos_CodigoEstablecimiento",
                table: "Establecimientos");

            migrationBuilder.DropIndex(
                name: "IX_ClientesEstablecimientos_ClienteId_EstablecimientoId",
                table: "ClientesEstablecimientos");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_CodigoCliente",
                table: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoSucursal",
                table: "Sucursales",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoEstablecimiento",
                table: "Establecimientos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoCliente",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_UsuarioId",
                table: "UsuariosSucursales",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_UsuarioId",
                table: "UsuariosEstablecimientos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId",
                table: "EstablecimientosEspecies",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_ClienteId",
                table: "ClientesEstablecimientos",
                column: "ClienteId");
        }
    }
}
