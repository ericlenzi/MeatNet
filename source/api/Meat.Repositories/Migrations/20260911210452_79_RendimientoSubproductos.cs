using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _79_RendimientoSubproductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RendimientosSubproductos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MaterialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Porcentaje = table.Column<double>(type: "float", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpresaId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RendimientosSubproductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RendimientosSubproductos_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_EmpresaId_EspecieId_MaterialId",
                table: "RendimientosSubproductos",
                columns: new[] { "EmpresaId", "EspecieId", "MaterialId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_EspecieId",
                table: "RendimientosSubproductos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_RendimientosSubproductos_MaterialId",
                table: "RendimientosSubproductos",
                column: "MaterialId");
            // Rendimientos iniciales de vacuno, sobre el peso de la res faenada: cuero 8%,
            // sebo 3% y menudencias 5%. Son los valores que la planta declaro para arrancar; se
            // ajustan desde la pantalla de Rendimientos de Subproductos.
            //
            // Se siembran buscando el material por su codigo dentro de cada empresa, porque el
            // catalogo de materiales es propio de cada una. La empresa que no tenga esos codigos
            // no recibe filas, y carga las suyas a mano.
            migrationBuilder.Sql(@"
                INSERT INTO RendimientosSubproductos (Id, EmpresaId, EspecieId, MaterialId, Porcentaje, Activo, FechaActualizacion)
                SELECT NEWID(), m.EmpresaId, 'V', m.Id,
                       CASE m.CodigoMaterial WHEN '4201' THEN 8 WHEN '4202' THEN 3 ELSE 5 END,
                       1, SYSDATETIME()
                FROM Materiales m
                WHERE m.CodigoMaterial IN ('4201', '4202', '4203')
                  AND m.FechaBaja IS NULL
                  AND NOT EXISTS (
                        SELECT 1 FROM RendimientosSubproductos r
                        WHERE r.EmpresaId = m.EmpresaId AND r.EspecieId = 'V'
                          AND r.MaterialId = m.Id AND r.FechaBaja IS NULL);
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RendimientosSubproductos");
        }
    }
}
