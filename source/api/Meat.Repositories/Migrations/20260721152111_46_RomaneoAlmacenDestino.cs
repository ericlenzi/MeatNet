using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _46_RomaneoAlmacenDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AlmacenDestinoId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Backfill de romaneos existentes: la camara destino no existia hasta ahora.
            // 1) Tomar la camara planificada en el renglon de la LM (default natural).
            migrationBuilder.Sql(@"
                UPDATE r SET r.AlmacenDestinoId = d.AlmacenDestinoId
                FROM Romaneos r
                INNER JOIN ListasMatanzasDetalles d ON r.ListaMatanzaDetalleId = d.Id
                WHERE d.AlmacenDestinoId IS NOT NULL;");
            // 2) Fallback: si el renglon no tenia camara, cualquier camara activa del establecimiento
            //    (garantiza que el FK NOT NULL no falle en filas heredadas).
            migrationBuilder.Sql(@"
                UPDATE r SET r.AlmacenDestinoId = c.Id
                FROM Romaneos r
                INNER JOIN ListasMatanzas lm ON r.ListaMatanzaId = lm.Id
                CROSS APPLY (
                    SELECT TOP 1 a.Id FROM Almacenes a
                    INNER JOIN TiposAlmacenes ta ON a.TipoAlmacenId = ta.Codigo
                    WHERE a.EstablecimientoId = lm.EstablecimientoId
                        AND ta.Familia = 'CAMARA' AND a.Activo = 1
                    ORDER BY a.Nombre
                ) c
                WHERE r.AlmacenDestinoId = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_AlmacenDestinoId",
                table: "Romaneos",
                column: "AlmacenDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Almacenes_AlmacenDestinoId",
                table: "Romaneos",
                column: "AlmacenDestinoId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Almacenes_AlmacenDestinoId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_AlmacenDestinoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "AlmacenDestinoId",
                table: "Romaneos");
        }
    }
}
