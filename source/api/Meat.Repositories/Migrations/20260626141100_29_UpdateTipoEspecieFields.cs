using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _29_UpdateTipoEspecieFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CodigoMaterialHasta",
                table: "TiposEspecies",
                newName: "ERP_Codigo");

            migrationBuilder.RenameColumn(
                name: "CodigoMaterialDesde",
                table: "TiposEspecies",
                newName: "CodigoMaterial");

            migrationBuilder.AddColumn<double>(
                name: "PesoTeorico",
                table: "TiposEspecies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PesoTeorico",
                table: "TiposEspecies");

            migrationBuilder.RenameColumn(
                name: "ERP_Codigo",
                table: "TiposEspecies",
                newName: "CodigoMaterialHasta");

            migrationBuilder.RenameColumn(
                name: "CodigoMaterial",
                table: "TiposEspecies",
                newName: "CodigoMaterialDesde");
        }
    }
}
