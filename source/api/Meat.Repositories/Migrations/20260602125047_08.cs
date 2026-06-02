using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParametrosSucursales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parametros",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Parametros");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Parametros",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Parametros",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Parametros",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parametros",
                table: "Parametros",
                column: "Codigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Parametros",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Parametros");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Parametros");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "Parametros",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Parametros",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Parametros",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parametros",
                table: "Parametros",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ParametrosSucursales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParametroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SucursalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosSucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParametrosSucursales_Parametros_ParametroId",
                        column: x => x.ParametroId,
                        principalTable: "Parametros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParametrosSucursales_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSucursales_ParametroId",
                table: "ParametrosSucursales",
                column: "ParametroId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametrosSucursales_SucursalId",
                table: "ParametrosSucursales",
                column: "SucursalId");
        }
    }
}
