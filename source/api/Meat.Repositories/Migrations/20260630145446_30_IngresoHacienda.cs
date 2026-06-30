using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _30_IngresoHacienda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EstablecimientoId",
                table: "Almacenes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill: asignar los almacenes existentes al primer establecimiento activo
            // (los almacenes eran globales; ahora pertenecen a un establecimiento).
            migrationBuilder.Sql(
                "UPDATE Almacenes SET EstablecimientoId = (SELECT TOP 1 Id FROM Establecimientos WHERE Activo = 1 ORDER BY CodigoEstablecimiento) " +
                "WHERE EstablecimientoId = '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.CreateTable(
                name: "TiposEstadosHacienda",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosHacienda", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosIngresos",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosIngresos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosTropas",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosTropas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroIngreso = table.Column<long>(type: "bigint", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaHoraIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumeroDte = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaEmisionDte = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProvinciaId = table.Column<int>(type: "int", nullable: false),
                    Localidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrigenHaciendaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsoHaciendaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Transportista = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Chofer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatenteCamion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatenteJaula = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PesoBruto = table.Column<double>(type: "float", nullable: false),
                    Tara = table.Column<double>(type: "float", nullable: false),
                    PesoNeto = table.Column<double>(type: "float", nullable: false),
                    EstadoIngresoId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioAprobacionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_ClientesEstablecimientos_ClienteEstablecimientoId",
                        column: x => x.ClienteEstablecimientoId,
                        principalTable: "ClientesEstablecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_OrigenesHaciendas_OrigenHaciendaId",
                        column: x => x.OrigenHaciendaId,
                        principalTable: "OrigenesHaciendas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_TiposEstadosIngresos_EstadoIngresoId",
                        column: x => x.EstadoIngresoId,
                        principalTable: "TiposEstadosIngresos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_UsosHaciendas_UsoHaciendaId",
                        column: x => x.UsoHaciendaId,
                        principalTable: "UsosHaciendas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendasPesadas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoEspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PesoIngreso = table.Column<double>(type: "float", nullable: false),
                    UnidadMedida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendasPesadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasPesadas_IngresosHaciendas_IngresoHaciendaId",
                        column: x => x.IngresoHaciendaId,
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasPesadas_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalTable: "TiposEspecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tropas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NumeroTropa = table.Column<long>(type: "bigint", nullable: false),
                    EstadoTropaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tropas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tropas_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tropas_IngresosHaciendas_IngresoHaciendaId",
                        column: x => x.IngresoHaciendaId,
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tropas_TiposEstadosTropas_EstadoTropaId",
                        column: x => x.EstadoTropaId,
                        principalTable: "TiposEstadosTropas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendasUbicaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoEspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AlmacenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PesoPromedio = table.Column<double>(type: "float", nullable: false),
                    EstadoHaciendaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendasUbicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_IngresosHaciendas_IngresoHaciendaId",
                        column: x => x.IngresoHaciendaId,
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalTable: "TiposEspecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_TiposEstadosHacienda_EstadoHaciendaId",
                        column: x => x.EstadoHaciendaId,
                        principalTable: "TiposEstadosHacienda",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_EstablecimientoId",
                table: "Almacenes",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ClienteEstablecimientoId",
                table: "IngresosHaciendas",
                column: "ClienteEstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ClienteId",
                table: "IngresosHaciendas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EstablecimientoId_NumeroIngreso",
                table: "IngresosHaciendas",
                columns: new[] { "EstablecimientoId", "NumeroIngreso" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EstadoIngresoId",
                table: "IngresosHaciendas",
                column: "EstadoIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_OrigenHaciendaId",
                table: "IngresosHaciendas",
                column: "OrigenHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ProvinciaId",
                table: "IngresosHaciendas",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_UsoHaciendaId",
                table: "IngresosHaciendas",
                column: "UsoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_IngresoHaciendaId",
                table: "IngresosHaciendasPesadas",
                column: "IngresoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_TipoEspecieId",
                table: "IngresosHaciendasPesadas",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_AlmacenId",
                table: "IngresosHaciendasUbicaciones",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_EstadoHaciendaId",
                table: "IngresosHaciendasUbicaciones",
                column: "EstadoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_IngresoHaciendaId",
                table: "IngresosHaciendasUbicaciones",
                column: "IngresoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_TipoEspecieId",
                table: "IngresosHaciendasUbicaciones",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_TropaId",
                table: "IngresosHaciendasUbicaciones",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_ClienteEstablecimientoId_EspecieCodigo_NumeroTropa",
                table: "Tropas",
                columns: new[] { "ClienteEstablecimientoId", "EspecieCodigo", "NumeroTropa" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EspecieCodigo",
                table: "Tropas",
                column: "EspecieCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EstadoTropaId",
                table: "Tropas",
                column: "EstadoTropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_IngresoHaciendaId",
                table: "Tropas",
                column: "IngresoHaciendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Almacenes_Establecimientos_EstablecimientoId",
                table: "Almacenes",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Seed catalogos de estado
            migrationBuilder.InsertData(
                table: "TiposEstadosIngresos",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "BORRADOR", "Borrador", true },
                    { "PENDIENTE", "Pendiente Aprobacion", true },
                    { "APROBADO", "Aprobado", true },
                    { "ANULADO", "Anulado", true },
                });

            migrationBuilder.InsertData(
                table: "TiposEstadosTropas",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "RECEPCIONADA", "Recepcionada", true },
                    { "ANULADA", "Anulada", true },
                });

            migrationBuilder.InsertData(
                table: "TiposEstadosHacienda",
                columns: new[] { "Codigo", "Nombre", "Activo" },
                values: new object[,]
                {
                    { "EN_PIE", "En Pie", true },
                    { "CAIDOS", "Caidos", true },
                    { "MUERTOS", "Muertos", true },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Almacenes_Establecimientos_EstablecimientoId",
                table: "Almacenes");

            migrationBuilder.DropTable(
                name: "IngresosHaciendasPesadas");

            migrationBuilder.DropTable(
                name: "IngresosHaciendasUbicaciones");

            migrationBuilder.DropTable(
                name: "TiposEstadosHacienda");

            migrationBuilder.DropTable(
                name: "Tropas");

            migrationBuilder.DropTable(
                name: "IngresosHaciendas");

            migrationBuilder.DropTable(
                name: "TiposEstadosTropas");

            migrationBuilder.DropTable(
                name: "TiposEstadosIngresos");

            migrationBuilder.DropIndex(
                name: "IX_Almacenes_EstablecimientoId",
                table: "Almacenes");

            migrationBuilder.DropColumn(
                name: "EstablecimientoId",
                table: "Almacenes");
        }
    }
}
