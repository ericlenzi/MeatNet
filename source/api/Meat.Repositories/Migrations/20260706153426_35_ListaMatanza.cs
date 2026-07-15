using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _35_ListaMatanza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposEstadosListasMatanzas",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosListasMatanzas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    NumeroLista = table.Column<long>(type: "bigint", nullable: false),
                    EstadoListaMatanzaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioConfirmacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaInicioEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_TiposEstadosListasMatanzas_EstadoListaMatanzaId",
                        column: x => x.EstadoListaMatanzaId,
                        principalTable: "TiposEstadosListasMatanzas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzasDetalles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Secuencia = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    CantidadFaenada = table.Column<int>(type: "int", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzasDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzasMovimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CantidadAnterior = table.Column<int>(type: "int", nullable: true),
                    CantidadNueva = table.Column<int>(type: "int", nullable: true),
                    SecuenciaAnterior = table.Column<int>(type: "int", nullable: true),
                    SecuenciaNueva = table.Column<int>(type: "int", nullable: true),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzasMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasMovimientos_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EspecieId",
                table: "ListasMatanzas",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "Fecha", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [EstadoListaMatanzaId] <> 'CANCELADA'");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_NumeroLista",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "NumeroLista" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstadoListaMatanzaId",
                table: "ListasMatanzas",
                column: "EstadoListaMatanzaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_AlmacenId",
                table: "ListasMatanzasDetalles",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_ListaMatanzaId",
                table: "ListasMatanzasDetalles",
                column: "ListaMatanzaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_TropaId",
                table: "ListasMatanzasDetalles",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasMovimientos_ListaMatanzaId",
                table: "ListasMatanzasMovimientos",
                column: "ListaMatanzaId");

            // Seed catalogo de estados de la Lista de Matanza
            migrationBuilder.InsertData(
                table: "TiposEstadosListasMatanzas",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "BORRADOR", "Borrador", true },
                    { "CONFIRMADA", "Confirmada", true },
                    { "EN_EJECUCION", "En Ejecucion", true },
                    { "FINALIZADA", "Finalizada", true },
                    { "CANCELADA", "Cancelada", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListasMatanzasDetalles");

            migrationBuilder.DropTable(
                name: "ListasMatanzasMovimientos");

            migrationBuilder.DropTable(
                name: "ListasMatanzas");

            migrationBuilder.DropTable(
                name: "TiposEstadosListasMatanzas");
        }
    }
}
