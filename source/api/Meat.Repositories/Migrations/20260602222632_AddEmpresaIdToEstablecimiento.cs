using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpresaIdToEstablecimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmpresaId",
                table: "Establecimientos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE Establecimientos SET EmpresaId = (SELECT TOP 1 Id FROM Empresas WHERE TipoEmpresaId = 'PRP') WHERE EmpresaId IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmpresaId",
                table: "Establecimientos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_EmpresaId",
                table: "Establecimientos",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Establecimientos_Empresas_EmpresaId",
                table: "Establecimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Establecimientos_Empresas_EmpresaId",
                table: "Establecimientos");

            migrationBuilder.DropIndex(
                name: "IX_Establecimientos_EmpresaId",
                table: "Establecimientos");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "Establecimientos");
        }
    }
}
