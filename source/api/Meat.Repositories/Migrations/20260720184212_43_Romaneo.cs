using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _43_Romaneo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Romaneos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListaMatanzaDetalleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnidadFaenaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroGarron = table.Column<int>(type: "int", nullable: false),
                    NumeroRomaneo = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Anulado = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Romaneos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Romaneos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_ListasMatanzasDetalles_ListaMatanzaDetalleId",
                        column: x => x.ListaMatanzaDetalleId,
                        principalTable: "ListasMatanzasDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_UnidadesFaenas_UnidadFaenaId",
                        column: x => x.UnidadFaenaId,
                        principalTable: "UnidadesFaenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomaneosPiezas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RomaneoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Letra = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipificacionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Peso = table.Column<double>(type: "float", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomaneosPiezas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Romaneos_RomaneoId",
                        column: x => x.RomaneoId,
                        principalTable: "Romaneos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Tipificaciones_TipificacionId",
                        column: x => x.TipificacionId,
                        principalTable: "Tipificaciones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomaneosPiezasMediciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RomaneoPiezaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMedicionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomaneosPiezasMediciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezasMediciones_RomaneosPiezas_RomaneoPiezaId",
                        column: x => x.RomaneoPiezaId,
                        principalTable: "RomaneosPiezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezasMediciones_TiposMediciones_TipoMedicionId",
                        column: x => x.TipoMedicionId,
                        principalTable: "TiposMediciones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EspecieId",
                table: "Romaneos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaDetalleId",
                table: "Romaneos",
                column: "ListaMatanzaDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaId_NumeroGarron",
                table: "Romaneos",
                columns: new[] { "ListaMatanzaId", "NumeroGarron" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [Anulado] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaId_NumeroRomaneo",
                table: "Romaneos",
                columns: new[] { "ListaMatanzaId", "NumeroRomaneo" },
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TropaId",
                table: "Romaneos",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_UnidadFaenaId",
                table: "Romaneos",
                column: "UnidadFaenaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_RomaneoId",
                table: "RomaneosPiezas",
                column: "RomaneoId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_TipificacionId",
                table: "RomaneosPiezas",
                column: "TipificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_RomaneoPiezaId",
                table: "RomaneosPiezasMediciones",
                column: "RomaneoPiezaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_TipoMedicionId",
                table: "RomaneosPiezasMediciones",
                column: "TipoMedicionId");

            // Nuevo estado de tropa para la Ejecucion de Faena (paso 3)
            migrationBuilder.InsertData(
                table: "TiposEstadosTropas",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "FAENADA", "Faenada", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposEstadosTropas",
                keyColumn: "Codigo",
                keyValue: "FAENADA");

            migrationBuilder.DropTable(
                name: "RomaneosPiezasMediciones");

            migrationBuilder.DropTable(
                name: "RomaneosPiezas");

            migrationBuilder.DropTable(
                name: "Romaneos");
        }
    }
}
