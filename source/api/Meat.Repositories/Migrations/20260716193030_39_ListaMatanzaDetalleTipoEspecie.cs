using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _39_ListaMatanzaDetalleTipoEspecie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // El renglon pasa a requerir TipoEspecie. Las listas de matanza existentes
            // (de prueba) no tienen categoria por renglon, por lo que se eliminan para
            // poder agregar la columna requerida con integridad referencial.
            migrationBuilder.Sql("DELETE FROM ListasMatanzasMovimientos;");
            migrationBuilder.Sql("DELETE FROM ListasMatanzasDetalles;");
            migrationBuilder.Sql("DELETE FROM ListasMatanzas;");

            migrationBuilder.AddColumn<string>(
                name: "TipoEspecieId",
                table: "ListasMatanzasDetalles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_TipoEspecieId",
                table: "ListasMatanzasDetalles",
                column: "TipoEspecieId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId",
                table: "ListasMatanzasDetalles",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId",
                table: "ListasMatanzasDetalles");

            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzasDetalles_TipoEspecieId",
                table: "ListasMatanzasDetalles");

            migrationBuilder.DropColumn(
                name: "TipoEspecieId",
                table: "ListasMatanzasDetalles");
        }
    }
}
