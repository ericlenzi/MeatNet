using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientesEstablecimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodigoRenspa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumeroCUIG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesEstablecimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientesEstablecimientos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientesEstablecimientos_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_ClienteId",
                table: "ClientesEstablecimientos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_EstablecimientoId",
                table: "ClientesEstablecimientos",
                column: "EstablecimientoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientesEstablecimientos");
        }
    }
}
