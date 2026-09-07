using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _53_RemoveCodigoMaterialTipoEspecieUnidadFaena : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoMaterial",
                table: "UnidadesFaenas");

            migrationBuilder.DropColumn(
                name: "CodigoMaterial",
                table: "TiposEspecies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoMaterial",
                table: "UnidadesFaenas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoMaterial",
                table: "TiposEspecies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
