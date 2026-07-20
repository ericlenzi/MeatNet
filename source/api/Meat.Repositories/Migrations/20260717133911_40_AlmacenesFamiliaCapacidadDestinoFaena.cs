using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _40_AlmacenesFamiliaCapacidadDestinoFaena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CantidadAnimales",
                table: "Almacenes",
                newName: "Capacidad");

            migrationBuilder.AddColumn<string>(
                name: "Familia",
                table: "TiposAlmacenes",
                type: "nvarchar(max)",
                nullable: true);

            // Familia de los tipos de corral ya existentes
            migrationBuilder.UpdateData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CORRAL_CAIDOS", column: "Familia", value: "CORRAL");
            migrationBuilder.UpdateData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CORRAL_MUERTOS", column: "Familia", value: "CORRAL");

            // Nuevos tipos: corral comun (flujo normal de faena) y camara de enfriamiento de faena
            migrationBuilder.InsertData(
                table: "TiposAlmacenes",
                columns: new[] { "Codigo", "Nombre", "Familia", "Activo" },
                values: new object[,]
                {
                    { "CORRAL_COMUN", "Corral Comun", "CORRAL", true },
                    { "CAMARA_FAENA", "Camara de Faena", "CAMARA", true },
                });

            // Data-fix: los corrales existentes sin tipo pasan a CORRAL_COMUN (antes eran el corral "no especial")
            migrationBuilder.Sql("UPDATE [Almacenes] SET [TipoAlmacenId] = 'CORRAL_COMUN' WHERE [TipoAlmacenId] IS NULL;");

            migrationBuilder.AddColumn<Guid>(
                name: "AlmacenDestinoId",
                table: "ListasMatanzasDetalles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_AlmacenDestinoId",
                table: "ListasMatanzasDetalles",
                column: "AlmacenDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzasDetalles_Almacenes_AlmacenDestinoId",
                table: "ListasMatanzasDetalles",
                column: "AlmacenDestinoId",
                principalTable: "Almacenes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListasMatanzasDetalles_Almacenes_AlmacenDestinoId",
                table: "ListasMatanzasDetalles");

            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzasDetalles_AlmacenDestinoId",
                table: "ListasMatanzasDetalles");

            // Revertir data-fix antes de borrar el tipo (evita violar la FK)
            migrationBuilder.Sql("UPDATE [Almacenes] SET [TipoAlmacenId] = NULL WHERE [TipoAlmacenId] = 'CORRAL_COMUN';");
            migrationBuilder.DeleteData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CORRAL_COMUN");
            migrationBuilder.DeleteData(table: "TiposAlmacenes", keyColumn: "Codigo", keyValue: "CAMARA_FAENA");

            migrationBuilder.DropColumn(
                name: "Familia",
                table: "TiposAlmacenes");

            migrationBuilder.DropColumn(
                name: "AlmacenDestinoId",
                table: "ListasMatanzasDetalles");

            migrationBuilder.RenameColumn(
                name: "Capacidad",
                table: "Almacenes",
                newName: "CantidadAnimales");
        }
    }
}
