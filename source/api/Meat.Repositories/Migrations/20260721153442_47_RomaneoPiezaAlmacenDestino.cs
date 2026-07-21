using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _47_RomaneoPiezaAlmacenDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La camara destino pasa de Romaneo (por animal) a RomaneoPieza (por media res).
            // 1) Nueva columna en la pieza.
            migrationBuilder.AddColumn<Guid>(
                name: "AlmacenDestinoId",
                table: "RomaneosPiezas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // 2) Backfill: cada pieza hereda la camara de su romaneo (aun presente en Romaneos).
            migrationBuilder.Sql(@"
                UPDATE p SET p.AlmacenDestinoId = r.AlmacenDestinoId
                FROM RomaneosPiezas p
                INNER JOIN Romaneos r ON p.RomaneoId = r.Id;");

            // 3) Recien ahora se elimina la camara a nivel Romaneo.
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Almacenes_AlmacenDestinoId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_AlmacenDestinoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "AlmacenDestinoId",
                table: "Romaneos");

            // 4) Indice + FK de la pieza (post-backfill: sin Guid.Empty que viole el FK).
            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_AlmacenDestinoId",
                table: "RomaneosPiezas",
                column: "AlmacenDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezas_Almacenes_AlmacenDestinoId",
                table: "RomaneosPiezas",
                column: "AlmacenDestinoId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RomaneosPiezas_Almacenes_AlmacenDestinoId",
                table: "RomaneosPiezas");

            migrationBuilder.DropIndex(
                name: "IX_RomaneosPiezas_AlmacenDestinoId",
                table: "RomaneosPiezas");

            migrationBuilder.DropColumn(
                name: "AlmacenDestinoId",
                table: "RomaneosPiezas");

            migrationBuilder.AddColumn<Guid>(
                name: "AlmacenDestinoId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_AlmacenDestinoId",
                table: "Romaneos",
                column: "AlmacenDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Almacenes_AlmacenDestinoId",
                table: "Romaneos",
                column: "AlmacenDestinoId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
