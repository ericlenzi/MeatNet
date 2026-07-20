using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _42_UnidadFaenaComplementariaInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnidadesFaenas_UnidadesFaenas_UnidadComplementariaId",
                table: "UnidadesFaenas");

            migrationBuilder.DropIndex(
                name: "IX_UnidadesFaenas_UnidadComplementariaId",
                table: "UnidadesFaenas");

            migrationBuilder.DropColumn(
                name: "UnidadComplementariaId",
                table: "UnidadesFaenas");

            migrationBuilder.AddColumn<int>(
                name: "UnidadComplementaria",
                table: "UnidadesFaenas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnidadComplementaria",
                table: "UnidadesFaenas");

            migrationBuilder.AddColumn<Guid>(
                name: "UnidadComplementariaId",
                table: "UnidadesFaenas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_UnidadComplementariaId",
                table: "UnidadesFaenas",
                column: "UnidadComplementariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesFaenas_UnidadesFaenas_UnidadComplementariaId",
                table: "UnidadesFaenas",
                column: "UnidadComplementariaId",
                principalTable: "UnidadesFaenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
