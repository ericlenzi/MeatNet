using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaIdToEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaId",
                table: "Empresas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_EmpresaId",
                table: "Empresas",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empresas_Empresas_EmpresaId",
                table: "Empresas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empresas_Empresas_EmpresaId",
                table: "Empresas");

            migrationBuilder.DropIndex(
                name: "IX_Empresas_EmpresaId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Empresas");
        }
    }
}
