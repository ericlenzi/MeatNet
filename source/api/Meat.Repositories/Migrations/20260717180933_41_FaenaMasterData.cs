using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _41_FaenaMasterData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puestos_Sucursales_SucursalId",
                table: "Puestos");

            migrationBuilder.RenameColumn(
                name: "SucursalId",
                table: "Puestos",
                newName: "EstablecimientoId");

            migrationBuilder.RenameIndex(
                name: "IX_Puestos_SucursalId",
                table: "Puestos",
                newName: "IX_Puestos_EstablecimientoId");

            migrationBuilder.AddColumn<string>(
                name: "TipoPuestoId",
                table: "Puestos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PuestoId",
                table: "ListasMatanzas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DestinosComerciales",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinosComerciales", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "MotivosDecomisos",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosDecomisos", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_MotivosDecomisos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Numeradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoNumerador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UltimoNumero = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numeradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Numeradores_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Numeradores_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TipificacionesOficiales",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipificacionesOficiales", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TipificacionesOficiales_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposContusiones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContusiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TiposContusiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposDenticiones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDenticiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TiposDenticiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposMediciones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMediciones", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposPuestos",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPuestos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesFaenas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CantidadCuartos = table.Column<int>(type: "int", nullable: false),
                    UnidadComplementariaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CodigoMaterial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ERP_Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesFaenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnidadesFaenas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesFaenas_UnidadesFaenas_UnidadComplementariaId",
                        column: x => x.UnidadComplementariaId,
                        principalTable: "UnidadesFaenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Denticiones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoDenticionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denticiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Denticiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Denticiones_TiposDenticiones_TipoDenticionId",
                        column: x => x.TipoDenticionId,
                        principalTable: "TiposDenticiones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tipificaciones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoEmpresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipoEspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UnidadFaenaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinoComercialId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TipificacionOficialId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PesoDesde = table.Column<double>(type: "float", nullable: false),
                    PesoHasta = table.Column<double>(type: "float", nullable: false),
                    UnidadMedidaId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipificaciones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_DestinosComerciales_DestinoComercialId",
                        column: x => x.DestinoComercialId,
                        principalTable: "DestinosComerciales",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_TipificacionesOficiales_TipificacionOficialId",
                        column: x => x.TipificacionOficialId,
                        principalTable: "TipificacionesOficiales",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalTable: "TiposEspecies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId",
                        column: x => x.UnidadFaenaId,
                        principalTable: "UnidadesFaenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_UnidadesMedidas_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalTable: "UnidadesMedidas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_TipoPuestoId",
                table: "Puestos",
                column: "TipoPuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_PuestoId",
                table: "ListasMatanzas",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Denticiones_EspecieId",
                table: "Denticiones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Denticiones_TipoDenticionId",
                table: "Denticiones",
                column: "TipoDenticionId");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosDecomisos_EspecieId",
                table: "MotivosDecomisos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EspecieCodigo",
                table: "Numeradores",
                column: "EspecieCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EstablecimientoId_EspecieCodigo_Codigo",
                table: "Numeradores",
                columns: new[] { "EstablecimientoId", "EspecieCodigo", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_DestinoComercialId",
                table: "Tipificaciones",
                column: "DestinoComercialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_EspecieId",
                table: "Tipificaciones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_TipificacionOficialId",
                table: "Tipificaciones",
                column: "TipificacionOficialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_TipoEspecieId",
                table: "Tipificaciones",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_UnidadFaenaId",
                table: "Tipificaciones",
                column: "UnidadFaenaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_UnidadMedidaId",
                table: "Tipificaciones",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_TipificacionesOficiales_EspecieId",
                table: "TipificacionesOficiales",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposContusiones_EspecieId",
                table: "TiposContusiones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposDenticiones_EspecieId",
                table: "TiposDenticiones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EspecieId_Numero",
                table: "UnidadesFaenas",
                columns: new[] { "EspecieId", "Numero" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_UnidadComplementariaId",
                table: "UnidadesFaenas",
                column: "UnidadComplementariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzas_Puestos_PuestoId",
                table: "ListasMatanzas",
                column: "PuestoId",
                principalTable: "Puestos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_Establecimientos_EstablecimientoId",
                table: "Puestos",
                column: "EstablecimientoId",
                principalTable: "Establecimientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_TiposPuestos_TipoPuestoId",
                table: "Puestos",
                column: "TipoPuestoId",
                principalTable: "TiposPuestos",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListasMatanzas_Puestos_PuestoId",
                table: "ListasMatanzas");

            migrationBuilder.DropForeignKey(
                name: "FK_Puestos_Establecimientos_EstablecimientoId",
                table: "Puestos");

            migrationBuilder.DropForeignKey(
                name: "FK_Puestos_TiposPuestos_TipoPuestoId",
                table: "Puestos");

            migrationBuilder.DropTable(
                name: "Denticiones");

            migrationBuilder.DropTable(
                name: "MotivosDecomisos");

            migrationBuilder.DropTable(
                name: "Numeradores");

            migrationBuilder.DropTable(
                name: "Tipificaciones");

            migrationBuilder.DropTable(
                name: "TiposContusiones");

            migrationBuilder.DropTable(
                name: "TiposMediciones");

            migrationBuilder.DropTable(
                name: "TiposPuestos");

            migrationBuilder.DropTable(
                name: "TiposDenticiones");

            migrationBuilder.DropTable(
                name: "DestinosComerciales");

            migrationBuilder.DropTable(
                name: "TipificacionesOficiales");

            migrationBuilder.DropTable(
                name: "UnidadesFaenas");

            migrationBuilder.DropIndex(
                name: "IX_Puestos_TipoPuestoId",
                table: "Puestos");

            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzas_PuestoId",
                table: "ListasMatanzas");

            migrationBuilder.DropColumn(
                name: "TipoPuestoId",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "PuestoId",
                table: "ListasMatanzas");

            migrationBuilder.RenameColumn(
                name: "EstablecimientoId",
                table: "Puestos",
                newName: "SucursalId");

            migrationBuilder.RenameIndex(
                name: "IX_Puestos_EstablecimientoId",
                table: "Puestos",
                newName: "IX_Puestos_SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_Sucursales_SucursalId",
                table: "Puestos",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
