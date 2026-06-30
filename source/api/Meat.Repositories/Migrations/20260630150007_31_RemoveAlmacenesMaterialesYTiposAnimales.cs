using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _31_RemoveAlmacenesMaterialesYTiposAnimales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlmacenesMateriales");

            migrationBuilder.DropTable(
                name: "TiposAnimales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposAnimales",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAnimales", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "AlmacenesMateriales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoAnimalId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlmacenesMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlmacenesMateriales_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlmacenesMateriales_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlmacenesMateriales_TiposAnimales_TipoAnimalId",
                        column: x => x.TipoAnimalId,
                        principalTable: "TiposAnimales",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenesMateriales_AlmacenId",
                table: "AlmacenesMateriales",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenesMateriales_MaterialId",
                table: "AlmacenesMateriales",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_AlmacenesMateriales_TipoAnimalId",
                table: "AlmacenesMateriales",
                column: "TipoAnimalId");
        }
    }
}
