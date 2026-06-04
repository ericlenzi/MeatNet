using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TiposEspecies");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoSexoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CodigoMaterialDesde = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoMaterialHasta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categorias_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                    table.ForeignKey(
                        name: "FK_Categorias_TiposSexos_TipoSexoId",
                        column: x => x.TipoSexoId,
                        principalTable: "TiposSexos",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_EspecieId",
                table: "Categorias",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_TipoSexoId",
                table: "Categorias",
                column: "TipoSexoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.CreateTable(
                name: "TiposEspecies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoSexoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    CodigoMaterialDesde = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoMaterialHasta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposEspecies_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                    table.ForeignKey(
                        name: "FK_TiposEspecies_TiposSexos_TipoSexoId",
                        column: x => x.TipoSexoId,
                        principalTable: "TiposSexos",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TiposEspecies_EspecieId",
                table: "TiposEspecies",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposEspecies_TipoSexoId",
                table: "TiposEspecies",
                column: "TipoSexoId");
        }
    }
}
