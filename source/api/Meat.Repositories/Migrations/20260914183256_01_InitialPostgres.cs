using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _01_InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "meat");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:citext", ",,");

            migrationBuilder.CreateTable(
                name: "Especies",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    RindeMinimo = table.Column<double>(type: "double precision", nullable: true),
                    RindeMaximo = table.Column<double>(type: "double precision", nullable: true),
                    MermaOreoReferencia = table.Column<double>(type: "double precision", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especies", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "OrigenesHaciendas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrigenesHaciendas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposAlmacenes",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Familia = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAlmacenes", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposClientes",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposClientes", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEmpresas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEmpresas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosHacienda",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosHacienda", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosIngresos",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosIngresos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosListasMatanzas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosListasMatanzas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposEstadosTropas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEstadosTropas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposMagnitudes",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMagnitudes", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposMateriales",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMateriales", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposMediciones",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMediciones", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposMovimientosCamaras",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMovimientosCamaras", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposPuestos",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposPuestos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "TiposSexos",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSexos", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedidas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedidas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "UsosHaciendas",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsosHaciendas", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "Conformaciones",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conformaciones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Conformaciones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Denticiones",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Denticiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Denticiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradosEngrasamiento",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradosEngrasamiento", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_GradosEngrasamiento_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MotivosDecomisos",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    ExigeContusion = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotivosDecomisos", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_MotivosDecomisos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TipificacionesOficiales",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipificacionesOficiales", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TipificacionesOficiales_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposContusiones",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContusiones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TiposContusiones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empresas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoEmpresaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroCuit = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroIngresosBrutos = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroInscripcionRuca = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    CodigoActividad = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Color = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Logo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_TiposEmpresas_TipoEmpresaId",
                        column: x => x.TipoEmpresaId,
                        principalSchema: "meat",
                        principalTable: "TiposEmpresas",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "TiposEspecies",
                schema: "meat",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoSexoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoTeoricoReferencia = table.Column<double>(type: "double precision", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEspecies", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_TiposEspecies_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TiposEspecies_TiposSexos_TipoSexoId",
                        column: x => x.TipoSexoId,
                        principalSchema: "meat",
                        principalTable: "TiposSexos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoCliente = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoClienteId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroCuit = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroIngresosBrutos = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clientes_TiposClientes_TipoClienteId",
                        column: x => x.TipoClienteId,
                        principalSchema: "meat",
                        principalTable: "TiposClientes",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "DestinosComerciales",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Favorito = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinosComerciales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinosComerciales_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Materiales",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoMaterial = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoMaterialId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UnidadMedidaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoTeorico = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materiales_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Materiales_TiposMateriales_TipoMaterialId",
                        column: x => x.TipoMaterialId,
                        principalSchema: "meat",
                        principalTable: "TiposMateriales",
                        principalColumn: "Codigo");
                    table.ForeignKey(
                        name: "FK_Materiales_UnidadesMedidas_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalSchema: "meat",
                        principalTable: "UnidadesMedidas",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "Parametros",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Valor = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parametros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parametros_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoSucursal = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Erp_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Direccion = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    CodigoPostal = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Localidad = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Provincia = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Zona = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Pais = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Color = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sucursales_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesFaenas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "citext", nullable: true),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    CantidadCuartos = table.Column<int>(type: "integer", nullable: false),
                    PiezasPorAnimal = table.Column<int>(type: "integer", nullable: false),
                    PorDefecto = table.Column<bool>(type: "boolean", nullable: false),
                    TipoMaterialId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesFaenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnidadesFaenas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesFaenas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnidadesFaenas_TiposMateriales_TipoMaterialId",
                        column: x => x.TipoMaterialId,
                        principalSchema: "meat",
                        principalTable: "TiposMateriales",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "citext", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Apellido = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Email = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Legajo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    RolId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalSchema: "meat",
                        principalTable: "Roles",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "EmpresasTiposEspecies",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    PesoTeorico = table.Column<double>(type: "double precision", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasTiposEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresasTiposEspecies_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresasTiposEspecies_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DespiecesMateriales",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialOrigenId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialDestinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Rendimiento = table.Column<double>(type: "double precision", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespiecesMateriales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespiecesMateriales_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DespiecesMateriales_Materiales_MaterialDestinoId",
                        column: x => x.MaterialDestinoId,
                        principalSchema: "meat",
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DespiecesMateriales_Materiales_MaterialOrigenId",
                        column: x => x.MaterialOrigenId,
                        principalSchema: "meat",
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RendimientosSubproductos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    Porcentaje = table.Column<double>(type: "double precision", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RendimientosSubproductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "meat",
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Establecimientos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoEstablecimiento = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroSenasa = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroRuca = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establecimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establecimientos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Establecimientos_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalSchema: "meat",
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tipificaciones",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "citext", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UnidadFaenaId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinoComercialId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipificacionOficialId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoDesde = table.Column<double>(type: "double precision", nullable: false),
                    PesoHasta = table.Column<double>(type: "double precision", nullable: false),
                    UnidadMedidaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Puntos = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_DestinosComerciales_DestinoComercialId",
                        column: x => x.DestinoComercialId,
                        principalSchema: "meat",
                        principalTable: "DestinosComerciales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "meat",
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_TipificacionesOficiales_TipificacionOficialId",
                        column: x => x.TipificacionOficialId,
                        principalSchema: "meat",
                        principalTable: "TipificacionesOficiales",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId",
                        column: x => x.UnidadFaenaId,
                        principalSchema: "meat",
                        principalTable: "UnidadesFaenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificaciones_UnidadesMedidas_UnidadMedidaId",
                        column: x => x.UnidadMedidaId,
                        principalSchema: "meat",
                        principalTable: "UnidadesMedidas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosSucursales",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    SucursalId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsMain = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosSucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosSucursales_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosSucursales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "meat",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Almacenes",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoAlmacen = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Capacidad = table.Column<int>(type: "integer", nullable: false),
                    TipoAlmacenId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Almacenes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Almacenes_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Almacenes_TiposAlmacenes_TipoAlmacenId",
                        column: x => x.TipoAlmacenId,
                        principalSchema: "meat",
                        principalTable: "TiposAlmacenes",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "ClientesEstablecimientos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoRenspa = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroCUIG = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesEstablecimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientesEstablecimientos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "meat",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientesEstablecimientos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientesEstablecimientos_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstablecimientosEspecies",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    MermaOreo = table.Column<double>(type: "double precision", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstablecimientosEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                    table.ForeignKey(
                        name: "FK_EstablecimientosEspecies_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Numeradores",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Codigo = table.Column<string>(type: "citext", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoNumerador = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UltimoNumero = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Numeradores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Numeradores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Numeradores_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Numeradores_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Puestos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CodigoPuesto = table.Column<string>(type: "citext", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoPuestoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoMedicionId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puestos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Puestos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puestos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puestos_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puestos_TiposMediciones_TipoMedicionId",
                        column: x => x.TipoMedicionId,
                        principalSchema: "meat",
                        principalTable: "TiposMediciones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Puestos_TiposPuestos_TipoPuestoId",
                        column: x => x.TipoPuestoId,
                        principalSchema: "meat",
                        principalTable: "TiposPuestos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tipificadores",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Matricula = table.Column<string>(type: "citext", nullable: true),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PorDefecto = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipificadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosEstablecimientos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsMain = table.Column<bool>(type: "boolean", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosEstablecimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosEstablecimientos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosEstablecimientos_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosEstablecimientos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalSchema: "meat",
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NumeroIngreso = table.Column<long>(type: "bigint", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    FechaHoraIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroDte = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaEmisionDte = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProvinciaId = table.Column<int>(type: "integer", nullable: false),
                    Localidad = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    OrigenHaciendaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UsoHaciendaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Transportista = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Chofer = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PatenteCamion = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PatenteJaula = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoNeto = table.Column<double>(type: "double precision", nullable: false),
                    EstadoIngresoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioAprobacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_ClientesEstablecimientos_ClienteEstableci~",
                        column: x => x.ClienteEstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "ClientesEstablecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalSchema: "meat",
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_OrigenesHaciendas_OrigenHaciendaId",
                        column: x => x.OrigenHaciendaId,
                        principalSchema: "meat",
                        principalTable: "OrigenesHaciendas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalSchema: "meat",
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_TiposEstadosIngresos_EstadoIngresoId",
                        column: x => x.EstadoIngresoId,
                        principalSchema: "meat",
                        principalTable: "TiposEstadosIngresos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendas_UsosHaciendas_UsoHaciendaId",
                        column: x => x.UsoHaciendaId,
                        principalSchema: "meat",
                        principalTable: "UsosHaciendas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumeradoresTropas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UltimoNumeroTropa = table.Column<long>(type: "bigint", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeradoresTropas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumeradoresTropas_ClientesEstablecimientos_ClienteEstableci~",
                        column: x => x.ClienteEstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "ClientesEstablecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumeradoresTropas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NumeradoresTropas_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PuestoId = table.Column<Guid>(type: "uuid", nullable: true),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    NumeroLista = table.Column<long>(type: "bigint", nullable: false),
                    EstadoListaMatanzaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioConfirmacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    FechaInicioEjecucion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaFinalizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_Puestos_PuestoId",
                        column: x => x.PuestoId,
                        principalSchema: "meat",
                        principalTable: "Puestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzas_TiposEstadosListasMatanzas_EstadoListaMatanz~",
                        column: x => x.EstadoListaMatanzaId,
                        principalSchema: "meat",
                        principalTable: "TiposEstadosListasMatanzas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendasPesadas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoIngreso = table.Column<double>(type: "double precision", nullable: false),
                    UnidadMedida = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    IdPesada = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendasPesadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasPesadas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasPesadas_IngresosHaciendas_IngresoHaciendaId",
                        column: x => x.IngresoHaciendaId,
                        principalSchema: "meat",
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasPesadas_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tropas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroTropa = table.Column<long>(type: "bigint", nullable: false),
                    EstadoTropaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tropas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tropas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tropas_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tropas_IngresosHaciendas_IngresoHaciendaId",
                        column: x => x.IngresoHaciendaId,
                        principalSchema: "meat",
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tropas_TiposEstadosTropas_EstadoTropaId",
                        column: x => x.EstadoTropaId,
                        principalSchema: "meat",
                        principalTable: "TiposEstadosTropas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzasMovimientos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: true),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: true),
                    CantidadAnterior = table.Column<int>(type: "integer", nullable: true),
                    CantidadNueva = table.Column<int>(type: "integer", nullable: true),
                    SecuenciaAnterior = table.Column<int>(type: "integer", nullable: true),
                    SecuenciaNueva = table.Column<int>(type: "integer", nullable: true),
                    Motivo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzasMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasMovimientos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasMovimientos_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalSchema: "meat",
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngresosHaciendasUbicaciones",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IngresoHaciendaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    PesoPromedio = table.Column<double>(type: "double precision", nullable: false),
                    EstadoHaciendaId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngresosHaciendasUbicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalSchema: "meat",
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_IngresosHaciendas_IngresoHacie~",
                        column: x => x.IngresoHaciendaId,
                        principalSchema: "meat",
                        principalTable: "IngresosHaciendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_TiposEstadosHacienda_EstadoHac~",
                        column: x => x.EstadoHaciendaId,
                        principalSchema: "meat",
                        principalTable: "TiposEstadosHacienda",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngresosHaciendasUbicaciones_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalSchema: "meat",
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListasMatanzasDetalles",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: false),
                    AlmacenDestinoId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: false, collation: "es-AR-x-icu"),
                    Secuencia = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    CantidadFaenada = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasMatanzasDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Almacenes_AlmacenDestinoId",
                        column: x => x.AlmacenDestinoId,
                        principalSchema: "meat",
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalSchema: "meat",
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalSchema: "meat",
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasMatanzasDetalles_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalSchema: "meat",
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TropasMovimientos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Secuencia = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EstadoResultanteId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Detalle = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    ReferenciaTipo = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    ReferenciaId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TropasMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TropasMovimientos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TropasMovimientos_TiposEstadosTropas_EstadoResultanteId",
                        column: x => x.EstadoResultanteId,
                        principalSchema: "meat",
                        principalTable: "TiposEstadosTropas",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TropasMovimientos_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalSchema: "meat",
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Romaneos",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListaMatanzaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ListaMatanzaDetalleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PuestoId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipificadorId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoMedicionId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    UnidadFaenaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConformacionId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    GradoEngrasamientoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    DenticionId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    DecomisoTotal = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoDecomisoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    NumeroGarron = table.Column<int>(type: "integer", nullable: false),
                    NumeroRomaneo = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Anulado = table.Column<bool>(type: "boolean", nullable: false),
                    Liberado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaLiberacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UsuarioLiberacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Romaneos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Romaneos_Conformaciones_ConformacionId",
                        column: x => x.ConformacionId,
                        principalSchema: "meat",
                        principalTable: "Conformaciones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Denticiones_DenticionId",
                        column: x => x.DenticionId,
                        principalSchema: "meat",
                        principalTable: "Denticiones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalSchema: "meat",
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_GradosEngrasamiento_GradoEngrasamientoId",
                        column: x => x.GradoEngrasamientoId,
                        principalSchema: "meat",
                        principalTable: "GradosEngrasamiento",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_ListasMatanzasDetalles_ListaMatanzaDetalleId",
                        column: x => x.ListaMatanzaDetalleId,
                        principalSchema: "meat",
                        principalTable: "ListasMatanzasDetalles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_ListasMatanzas_ListaMatanzaId",
                        column: x => x.ListaMatanzaId,
                        principalSchema: "meat",
                        principalTable: "ListasMatanzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_MotivosDecomisos_MotivoDecomisoId",
                        column: x => x.MotivoDecomisoId,
                        principalSchema: "meat",
                        principalTable: "MotivosDecomisos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Puestos_PuestoId",
                        column: x => x.PuestoId,
                        principalSchema: "meat",
                        principalTable: "Puestos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Tipificadores_TipificadorId",
                        column: x => x.TipificadorId,
                        principalSchema: "meat",
                        principalTable: "Tipificadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_TiposMediciones_TipoMedicionId",
                        column: x => x.TipoMedicionId,
                        principalSchema: "meat",
                        principalTable: "TiposMediciones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalSchema: "meat",
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Romaneos_UnidadesFaenas_UnidadFaenaId",
                        column: x => x.UnidadFaenaId,
                        principalSchema: "meat",
                        principalTable: "UnidadesFaenas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomaneosPiezas",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RomaneoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Letra = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    AlmacenDestinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipificacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TipoContusionId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    PesoFueraRango = table.Column<bool>(type: "boolean", nullable: false),
                    Decomisada = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoDecomisoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    PesoDecomisado = table.Column<double>(type: "double precision", nullable: false),
                    Liberado = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomaneosPiezas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Almacenes_AlmacenDestinoId",
                        column: x => x.AlmacenDestinoId,
                        principalSchema: "meat",
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_MotivosDecomisos_MotivoDecomisoId",
                        column: x => x.MotivoDecomisoId,
                        principalSchema: "meat",
                        principalTable: "MotivosDecomisos",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Romaneos_RomaneoId",
                        column: x => x.RomaneoId,
                        principalSchema: "meat",
                        principalTable: "Romaneos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_Tipificaciones_TipificacionId",
                        column: x => x.TipificacionId,
                        principalSchema: "meat",
                        principalTable: "Tipificaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezas_TiposContusiones_TipoContusionId",
                        column: x => x.TipoContusionId,
                        principalSchema: "meat",
                        principalTable: "TiposContusiones",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosCamaras",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlmacenId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaterialId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMovimientoId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    RomaneoPiezaOrigenId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransformacionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TropaId = table.Column<Guid>(type: "uuid", nullable: true),
                    EspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    TipoEspecieId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uuid", nullable: true),
                    Referencia = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosCamaras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalSchema: "meat",
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalSchema: "meat",
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalSchema: "meat",
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_RomaneosPiezas_RomaneoPiezaOrigenId",
                        column: x => x.RomaneoPiezaOrigenId,
                        principalSchema: "meat",
                        principalTable: "RomaneosPiezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalSchema: "meat",
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_TiposMovimientosCamaras_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalSchema: "meat",
                        principalTable: "TiposMovimientosCamaras",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosCamaras_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalSchema: "meat",
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RomaneosPiezasMediciones",
                schema: "meat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RomaneoPiezaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoMagnitudId = table.Column<string>(type: "text", nullable: true, collation: "es-AR-x-icu"),
                    Valor = table.Column<double>(type: "double precision", nullable: false),
                    EmpresaId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, collation: "es-AR-x-icu"),
                    FechaBaja = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RomaneosPiezasMediciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezasMediciones_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalSchema: "meat",
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezasMediciones_RomaneosPiezas_RomaneoPiezaId",
                        column: x => x.RomaneoPiezaId,
                        principalSchema: "meat",
                        principalTable: "RomaneosPiezas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RomaneosPiezasMediciones_TiposMagnitudes_TipoMagnitudId",
                        column: x => x.TipoMagnitudId,
                        principalSchema: "meat",
                        principalTable: "TiposMagnitudes",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_EmpresaId",
                schema: "meat",
                table: "Almacenes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_EstablecimientoId",
                schema: "meat",
                table: "Almacenes",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_TipoAlmacenId",
                schema: "meat",
                table: "Almacenes",
                column: "TipoAlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresaId_CodigoCliente",
                schema: "meat",
                table: "Clientes",
                columns: new[] { "EmpresaId", "CodigoCliente" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_TipoClienteId",
                schema: "meat",
                table: "Clientes",
                column: "TipoClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_ClienteId_EstablecimientoId",
                schema: "meat",
                table: "ClientesEstablecimientos",
                columns: new[] { "ClienteId", "EstablecimientoId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_EmpresaId",
                schema: "meat",
                table: "ClientesEstablecimientos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_EstablecimientoId",
                schema: "meat",
                table: "ClientesEstablecimientos",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Conformaciones_EspecieId",
                schema: "meat",
                table: "Conformaciones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Denticiones_EspecieId",
                schema: "meat",
                table: "Denticiones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_EmpresaId",
                schema: "meat",
                table: "DespiecesMateriales",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_MaterialDestinoId",
                schema: "meat",
                table: "DespiecesMateriales",
                column: "MaterialDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_MaterialOrigenId_MaterialDestinoId",
                schema: "meat",
                table: "DespiecesMateriales",
                columns: new[] { "MaterialOrigenId", "MaterialDestinoId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DestinosComerciales_EmpresaId_Codigo",
                schema: "meat",
                table: "DestinosComerciales",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DestinosComerciales_EmpresaId_Favorito",
                schema: "meat",
                table: "DestinosComerciales",
                columns: new[] { "EmpresaId", "Favorito" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL AND \"Favorito\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_TipoEmpresaId",
                schema: "meat",
                table: "Empresas",
                column: "TipoEmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasTiposEspecies_EmpresaId_TipoEspecieId",
                schema: "meat",
                table: "EmpresasTiposEspecies",
                columns: new[] { "EmpresaId", "TipoEspecieId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasTiposEspecies_TipoEspecieId",
                schema: "meat",
                table: "EmpresasTiposEspecies",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_EmpresaId_CodigoEstablecimiento",
                schema: "meat",
                table: "Establecimientos",
                columns: new[] { "EmpresaId", "CodigoEstablecimiento" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_SucursalId",
                schema: "meat",
                table: "Establecimientos",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EmpresaId",
                schema: "meat",
                table: "EstablecimientosEspecies",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EspecieId",
                schema: "meat",
                table: "EstablecimientosEspecies",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EstablecimientoId_EspecieId",
                schema: "meat",
                table: "EstablecimientosEspecies",
                columns: new[] { "EstablecimientoId", "EspecieId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GradosEngrasamiento_EspecieId",
                schema: "meat",
                table: "GradosEngrasamiento",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ClienteEstablecimientoId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "ClienteEstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ClienteId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EmpresaId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EspecieId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EstablecimientoId_NumeroIngreso",
                schema: "meat",
                table: "IngresosHaciendas",
                columns: new[] { "EstablecimientoId", "NumeroIngreso" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EstadoIngresoId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "EstadoIngresoId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_OrigenHaciendaId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "OrigenHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_ProvinciaId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_UsoHaciendaId",
                schema: "meat",
                table: "IngresosHaciendas",
                column: "UsoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_EmpresaId",
                schema: "meat",
                table: "IngresosHaciendasPesadas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_IngresoHaciendaId",
                schema: "meat",
                table: "IngresosHaciendasPesadas",
                column: "IngresoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_TipoEspecieId",
                schema: "meat",
                table: "IngresosHaciendasPesadas",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_AlmacenId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_EmpresaId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_EstadoHaciendaId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "EstadoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_IngresoHaciendaId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "IngresoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_TipoEspecieId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_TropaId",
                schema: "meat",
                table: "IngresosHaciendasUbicaciones",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EmpresaId",
                schema: "meat",
                table: "ListasMatanzas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EspecieId",
                schema: "meat",
                table: "ListasMatanzas",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_EspecieId_NumeroLista",
                schema: "meat",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "EspecieId", "NumeroLista" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_Fecha_EspecieId",
                schema: "meat",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "Fecha", "EspecieId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL AND \"EstadoListaMatanzaId\" <> 'ANULADA'");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstadoListaMatanzaId",
                schema: "meat",
                table: "ListasMatanzas",
                column: "EstadoListaMatanzaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_PuestoId",
                schema: "meat",
                table: "ListasMatanzas",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_AlmacenDestinoId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "AlmacenDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_AlmacenId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_EmpresaId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_ListaMatanzaId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "ListaMatanzaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_TipoEspecieId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_TropaId",
                schema: "meat",
                table: "ListasMatanzasDetalles",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasMovimientos_EmpresaId",
                schema: "meat",
                table: "ListasMatanzasMovimientos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasMovimientos_ListaMatanzaId",
                schema: "meat",
                table: "ListasMatanzasMovimientos",
                column: "ListaMatanzaId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_EmpresaId_CodigoMaterial",
                schema: "meat",
                table: "Materiales",
                columns: new[] { "EmpresaId", "CodigoMaterial" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_TipoMaterialId",
                schema: "meat",
                table: "Materiales",
                column: "TipoMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Materiales_UnidadMedidaId",
                schema: "meat",
                table: "Materiales",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_MotivosDecomisos_EspecieId",
                schema: "meat",
                table: "MotivosDecomisos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_AlmacenId_MaterialId",
                schema: "meat",
                table: "MovimientosCamaras",
                columns: new[] { "AlmacenId", "MaterialId" },
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_EmpresaId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_EspecieId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_MaterialId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_RomaneoPiezaOrigenId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "RomaneoPiezaOrigenId",
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TipoEspecieId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TipoMovimientoId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_TropaId",
                schema: "meat",
                table: "MovimientosCamaras",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EmpresaId",
                schema: "meat",
                table: "Numeradores",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EspecieCodigo",
                schema: "meat",
                table: "Numeradores",
                column: "EspecieCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EstablecimientoId_EspecieCodigo_Codigo",
                schema: "meat",
                table: "Numeradores",
                columns: new[] { "EstablecimientoId", "EspecieCodigo", "Codigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId_EspecieCodigo",
                schema: "meat",
                table: "NumeradoresTropas",
                columns: new[] { "ClienteEstablecimientoId", "EspecieCodigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_EmpresaId",
                schema: "meat",
                table: "NumeradoresTropas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_EspecieCodigo",
                schema: "meat",
                table: "NumeradoresTropas",
                column: "EspecieCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Parametros_EmpresaId_Codigo",
                schema: "meat",
                table: "Parametros",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EmpresaId_CodigoPuesto",
                schema: "meat",
                table: "Puestos",
                columns: new[] { "EmpresaId", "CodigoPuesto" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EspecieId",
                schema: "meat",
                table: "Puestos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EstablecimientoId",
                schema: "meat",
                table: "Puestos",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_TipoMedicionId",
                schema: "meat",
                table: "Puestos",
                column: "TipoMedicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_TipoPuestoId",
                schema: "meat",
                table: "Puestos",
                column: "TipoPuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_EmpresaId_EspecieId_MaterialId",
                schema: "meat",
                table: "RendimientosSubproductos",
                columns: new[] { "EmpresaId", "EspecieId", "MaterialId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_EspecieId",
                schema: "meat",
                table: "RendimientosSubproductos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_MaterialId",
                schema: "meat",
                table: "RendimientosSubproductos",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ConformacionId",
                schema: "meat",
                table: "Romaneos",
                column: "ConformacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_DenticionId",
                schema: "meat",
                table: "Romaneos",
                column: "DenticionId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EmpresaId",
                schema: "meat",
                table: "Romaneos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EspecieId",
                schema: "meat",
                table: "Romaneos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EstablecimientoId_EspecieId_NumeroRomaneo",
                schema: "meat",
                table: "Romaneos",
                columns: new[] { "EstablecimientoId", "EspecieId", "NumeroRomaneo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_GradoEngrasamientoId",
                schema: "meat",
                table: "Romaneos",
                column: "GradoEngrasamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaDetalleId",
                schema: "meat",
                table: "Romaneos",
                column: "ListaMatanzaDetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaId_NumeroGarron",
                schema: "meat",
                table: "Romaneos",
                columns: new[] { "ListaMatanzaId", "NumeroGarron" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL AND \"Anulado\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ListaMatanzaId_NumeroRomaneo",
                schema: "meat",
                table: "Romaneos",
                columns: new[] { "ListaMatanzaId", "NumeroRomaneo" },
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_MotivoDecomisoId",
                schema: "meat",
                table: "Romaneos",
                column: "MotivoDecomisoId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_PuestoId",
                schema: "meat",
                table: "Romaneos",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TipificadorId",
                schema: "meat",
                table: "Romaneos",
                column: "TipificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TipoMedicionId",
                schema: "meat",
                table: "Romaneos",
                column: "TipoMedicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TropaId",
                schema: "meat",
                table: "Romaneos",
                column: "TropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_UnidadFaenaId",
                schema: "meat",
                table: "Romaneos",
                column: "UnidadFaenaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_AlmacenDestinoId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "AlmacenDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_EmpresaId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_MotivoDecomisoId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "MotivoDecomisoId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_RomaneoId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "RomaneoId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_TipificacionId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "TipificacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_TipoContusionId",
                schema: "meat",
                table: "RomaneosPiezas",
                column: "TipoContusionId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_EmpresaId",
                schema: "meat",
                table: "RomaneosPiezasMediciones",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_RomaneoPiezaId",
                schema: "meat",
                table: "RomaneosPiezasMediciones",
                column: "RomaneoPiezaId");

            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_TipoMagnitudId",
                schema: "meat",
                table: "RomaneosPiezasMediciones",
                column: "TipoMagnitudId");

            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_EmpresaId_CodigoSucursal",
                schema: "meat",
                table: "Sucursales",
                columns: new[] { "EmpresaId", "CodigoSucursal" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_DestinoComercialId",
                schema: "meat",
                table: "Tipificaciones",
                column: "DestinoComercialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_EmpresaId_Codigo",
                schema: "meat",
                table: "Tipificaciones",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_EspecieId",
                schema: "meat",
                table: "Tipificaciones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_MaterialId",
                schema: "meat",
                table: "Tipificaciones",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_TipificacionOficialId",
                schema: "meat",
                table: "Tipificaciones",
                column: "TipificacionOficialId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_TipoEspecieId",
                schema: "meat",
                table: "Tipificaciones",
                column: "TipoEspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_UnidadFaenaId",
                schema: "meat",
                table: "Tipificaciones",
                column: "UnidadFaenaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_UnidadMedidaId",
                schema: "meat",
                table: "Tipificaciones",
                column: "UnidadMedidaId");

            migrationBuilder.CreateIndex(
                name: "IX_TipificacionesOficiales_EspecieId",
                schema: "meat",
                table: "TipificacionesOficiales",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EmpresaId_Matricula",
                schema: "meat",
                table: "Tipificadores",
                columns: new[] { "EmpresaId", "Matricula" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EspecieId",
                schema: "meat",
                table: "Tipificadores",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EstablecimientoId_EspecieId",
                schema: "meat",
                table: "Tipificadores",
                columns: new[] { "EstablecimientoId", "EspecieId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL AND \"PorDefecto\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_TiposContusiones_EspecieId",
                schema: "meat",
                table: "TiposContusiones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposEspecies_EspecieId",
                schema: "meat",
                table: "TiposEspecies",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposEspecies_TipoSexoId",
                schema: "meat",
                table: "TiposEspecies",
                column: "TipoSexoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_ClienteEstablecimientoId_EspecieCodigo_NumeroTropa",
                schema: "meat",
                table: "Tropas",
                columns: new[] { "ClienteEstablecimientoId", "EspecieCodigo", "NumeroTropa" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EmpresaId",
                schema: "meat",
                table: "Tropas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EspecieCodigo",
                schema: "meat",
                table: "Tropas",
                column: "EspecieCodigo");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EstadoTropaId",
                schema: "meat",
                table: "Tropas",
                column: "EstadoTropaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tropas_IngresoHaciendaId",
                schema: "meat",
                table: "Tropas",
                column: "IngresoHaciendaId");

            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_EmpresaId",
                schema: "meat",
                table: "TropasMovimientos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_EstadoResultanteId",
                schema: "meat",
                table: "TropasMovimientos",
                column: "EstadoResultanteId");

            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_TropaId_Secuencia",
                schema: "meat",
                table: "TropasMovimientos",
                columns: new[] { "TropaId", "Secuencia" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EmpresaId_Codigo",
                schema: "meat",
                table: "UnidadesFaenas",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EmpresaId_EspecieId",
                schema: "meat",
                table: "UnidadesFaenas",
                columns: new[] { "EmpresaId", "EspecieId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL AND \"PorDefecto\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EspecieId",
                schema: "meat",
                table: "UnidadesFaenas",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_TipoMaterialId",
                schema: "meat",
                table: "UnidadesFaenas",
                column: "TipoMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId",
                schema: "meat",
                table: "Usuarios",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                schema: "meat",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_UserName",
                schema: "meat",
                table: "Usuarios",
                column: "UserName",
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_EmpresaId",
                schema: "meat",
                table: "UsuariosEstablecimientos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_EstablecimientoId",
                schema: "meat",
                table: "UsuariosEstablecimientos",
                column: "EstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_UsuarioId_EstablecimientoId",
                schema: "meat",
                table: "UsuariosEstablecimientos",
                columns: new[] { "UsuarioId", "EstablecimientoId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_EmpresaId",
                schema: "meat",
                table: "UsuariosSucursales",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_UsuarioId_SucursalId",
                schema: "meat",
                table: "UsuariosSucursales",
                columns: new[] { "UsuarioId", "SucursalId" },
                unique: true,
                filter: "\"FechaBaja\" IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DespiecesMateriales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "EmpresasTiposEspecies",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "EstablecimientosEspecies",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "IngresosHaciendasPesadas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "IngresosHaciendasUbicaciones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "ListasMatanzasMovimientos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "MovimientosCamaras",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Numeradores",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "NumeradoresTropas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Parametros",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "RendimientosSubproductos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "RomaneosPiezasMediciones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TropasMovimientos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "UsuariosEstablecimientos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "UsuariosSucursales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEstadosHacienda",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposMovimientosCamaras",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "RomaneosPiezas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposMagnitudes",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Romaneos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Tipificaciones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposContusiones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Conformaciones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Denticiones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "GradosEngrasamiento",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "ListasMatanzasDetalles",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "MotivosDecomisos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Tipificadores",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "DestinosComerciales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Materiales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TipificacionesOficiales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "UnidadesFaenas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Almacenes",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "ListasMatanzas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEspecies",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Tropas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "UnidadesMedidas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposMateriales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposAlmacenes",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Puestos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEstadosListasMatanzas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposSexos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "IngresosHaciendas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEstadosTropas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposMediciones",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposPuestos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "ClientesEstablecimientos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Especies",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "OrigenesHaciendas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Provincias",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEstadosIngresos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "UsosHaciendas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Clientes",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Establecimientos",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposClientes",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Sucursales",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "Empresas",
                schema: "meat");

            migrationBuilder.DropTable(
                name: "TiposEmpresas",
                schema: "meat");
        }
    }
}
