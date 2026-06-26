using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _26_AddNumeradoresTropas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NumeradoresTropas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteEstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieCodigo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UltimoNumeroTropa = table.Column<long>(type: "bigint", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumeradoresTropas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumeradoresTropas_ClientesEstablecimientos_ClienteEstablecimientoId",
                        column: x => x.ClienteEstablecimientoId,
                        principalTable: "ClientesEstablecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumeradoresTropas_Especies_EspecieCodigo",
                        column: x => x.EspecieCodigo,
                        principalTable: "Especies",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_ClienteEstablecimientoId",
                table: "NumeradoresTropas",
                column: "ClienteEstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_EspecieCodigo",
                table: "NumeradoresTropas",
                column: "EspecieCodigo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NumeradoresTropas");
        }
    }
}
