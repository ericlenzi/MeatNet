using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _37_TropaMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TropasMovimientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TropaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Secuencia = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoResultanteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenciaTipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TropasMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TropasMovimientos_Tropas_TropaId",
                        column: x => x.TropaId,
                        principalTable: "Tropas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_TropaId_Secuencia",
                table: "TropasMovimientos",
                columns: new[] { "TropaId", "Secuencia" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TropasMovimientos");
        }
    }
}
