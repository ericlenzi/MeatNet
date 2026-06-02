using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaIdToRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add column as nullable
            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: true);

            // 2. Update existing rows with the first PRP empresa
            migrationBuilder.Sql(
                "UPDATE Roles SET EmpresaId = (SELECT TOP 1 Id FROM Empresas WHERE TipoEmpresaId = 'PRP') WHERE EmpresaId IS NULL");

            // 3. Make column not null
            migrationBuilder.AlterColumn<Guid>(
                name: "EmpresaId",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EmpresaId",
                table: "Roles",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Empresas_EmpresaId",
                table: "Roles",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Empresas_EmpresaId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_EmpresaId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Roles");
        }
    }
}
