using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _66_EjesTipificacionOficial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConformacionId",
                table: "Romaneos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GradoEngrasamientoId",
                table: "Romaneos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Conformaciones",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conformaciones", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_Conformaciones_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GradosEngrasamiento",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradosEngrasamiento", x => x.Codigo);
                    table.ForeignKey(
                        name: "FK_GradosEngrasamiento_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_ConformacionId",
                table: "Romaneos",
                column: "ConformacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_GradoEngrasamientoId",
                table: "Romaneos",
                column: "GradoEngrasamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Conformaciones_EspecieId",
                table: "Conformaciones",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_GradosEngrasamiento_EspecieId",
                table: "GradosEngrasamiento",
                column: "EspecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Conformaciones_ConformacionId",
                table: "Romaneos",
                column: "ConformacionId",
                principalTable: "Conformaciones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_GradosEngrasamiento_GradoEngrasamientoId",
                table: "Romaneos",
                column: "GradoEngrasamientoId",
                principalTable: "GradosEngrasamiento",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Conformaciones_ConformacionId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_GradosEngrasamiento_GradoEngrasamientoId",
                table: "Romaneos");

            migrationBuilder.DropTable(
                name: "Conformaciones");

            migrationBuilder.DropTable(
                name: "GradosEngrasamiento");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_ConformacionId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_GradoEngrasamientoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "ConformacionId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "GradoEngrasamientoId",
                table: "Romaneos");
        }
    }
}
