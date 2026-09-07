using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _57_DespieceMaterialYSeedUnidadesMedidas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DespiecesMateriales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialOrigenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialDestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Rendimiento = table.Column<double>(type: "float", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespiecesMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespiecesMateriales_Materiales_MaterialDestinoId",
                        column: x => x.MaterialDestinoId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DespiecesMateriales_Materiales_MaterialOrigenId",
                        column: x => x.MaterialOrigenId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_MaterialDestinoId",
                table: "DespiecesMateriales",
                column: "MaterialDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_MaterialOrigenId_MaterialDestinoId",
                table: "DespiecesMateriales",
                columns: new[] { "MaterialOrigenId", "MaterialDestinoId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            // Seed de unidades de medida basicas (KG / UN), idempotente por si ya existen.
            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT 1 FROM UnidadesMedidas WHERE Codigo = 'KG') " +
                "INSERT INTO UnidadesMedidas (Codigo, Nombre, Activo) VALUES ('KG', 'Kilogramo', 1);");
            migrationBuilder.Sql(
                "IF NOT EXISTS (SELECT 1 FROM UnidadesMedidas WHERE Codigo = 'UN') " +
                "INSERT INTO UnidadesMedidas (Codigo, Nombre, Activo) VALUES ('UN', 'Unidad', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DespiecesMateriales");

            migrationBuilder.Sql("DELETE FROM UnidadesMedidas WHERE Codigo IN ('KG', 'UN');");
        }
    }
}
