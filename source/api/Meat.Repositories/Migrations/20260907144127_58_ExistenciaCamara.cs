using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _58_ExistenciaCamara : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Liberado",
                table: "RomaneosPiezas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLiberacion",
                table: "Romaneos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Liberado",
                table: "Romaneos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UsuarioLiberacionId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposMovimientosCamaras",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMovimientosCamaras", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosCamaras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoMovimientoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    Peso = table.Column<double>(type: "float", nullable: false),
                    RomaneoPiezaOrigenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TransformacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoEspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Referencia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosCamaras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_RomaneosPiezas_RomaneoPiezaOrigenId",
                        column: x => x.RomaneoPiezaOrigenId,
                        principalTable: "RomaneosPiezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalTable: "TiposEspecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_TiposMovimientosCamaras_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalTable: "TiposMovimientosCamaras",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_AlmacenId_MaterialId",
                table: "MovimientosCamaras",
                columns: new[] { "AlmacenId", "MaterialId" },
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_EspecieId",
                table: "MovimientosCamaras",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_MaterialId",
                table: "MovimientosCamaras",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_RomaneoPiezaOrigenId",
                table: "MovimientosCamaras",
                column: "RomaneoPiezaOrigenId",
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TipoEspecieId",
                table: "MovimientosCamaras",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TipoMovimientoId",
                table: "MovimientosCamaras",
                column: "TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TropaId",
                table: "MovimientosCamaras",
                column: "TropaId");

            // Catalogo de tipos de movimiento de camara. Son dependencia dura del codigo
            // (la Liberacion escribe INGRESO o el par TRANSF_BAJA/TRANSF_ALTA), por eso se
            // siembran por sistema y no los carga el usuario. EGRESO lo usara el Ciclo II.
            migrationBuilder.InsertData(
                table: "TiposMovimientosCamaras",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "INGRESO", "Ingreso por liberacion", true },
                    { "TRANSF_BAJA", "Baja por transformacion", true },
                    { "TRANSF_ALTA", "Alta por transformacion", true },
                    { "EGRESO", "Egreso a Ciclo II", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosCamaras");

            migrationBuilder.DropTable(
                name: "TiposMovimientosCamaras");

            migrationBuilder.DropColumn(
                name: "Liberado",
                table: "RomaneosPiezas");

            migrationBuilder.DropColumn(
                name: "FechaLiberacion",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "Liberado",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "UsuarioLiberacionId",
                table: "Romaneos");
        }
    }
}
