using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _56_TipificacionMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MaterialId",
                table: "Tipificaciones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_MaterialId",
                table: "Tipificaciones",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_Materiales_MaterialId",
                table: "Tipificaciones",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tipificaciones_Materiales_MaterialId",
                table: "Tipificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Tipificaciones_MaterialId",
                table: "Tipificaciones");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "Tipificaciones");
        }
    }
}
