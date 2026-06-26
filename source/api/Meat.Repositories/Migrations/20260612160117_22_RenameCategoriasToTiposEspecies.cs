using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _22_RenameCategoriasToTiposEspecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Categorias",
                newName: "TiposEspecies");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_EspecieId",
                table: "TiposEspecies",
                newName: "IX_TiposEspecies_EspecieId");

            migrationBuilder.RenameIndex(
                name: "IX_Categorias_TipoSexoId",
                table: "TiposEspecies",
                newName: "IX_TiposEspecies_TipoSexoId");

            migrationBuilder.Sql("EXEC sp_rename N'PK_Categorias', N'PK_TiposEspecies';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_Categorias_Especies_EspecieId', N'FK_TiposEspecies_Especies_EspecieId';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_Categorias_TiposSexos_TipoSexoId', N'FK_TiposEspecies_TiposSexos_TipoSexoId';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_TiposEspecies_EspecieId",
                table: "TiposEspecies",
                newName: "IX_Categorias_EspecieId");

            migrationBuilder.RenameIndex(
                name: "IX_TiposEspecies_TipoSexoId",
                table: "TiposEspecies",
                newName: "IX_Categorias_TipoSexoId");

            migrationBuilder.Sql("EXEC sp_rename N'PK_TiposEspecies', N'PK_Categorias';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_TiposEspecies_Especies_EspecieId', N'FK_Categorias_Especies_EspecieId';");
            migrationBuilder.Sql("EXEC sp_rename N'FK_TiposEspecies_TiposSexos_TipoSexoId', N'FK_Categorias_TiposSexos_TipoSexoId';");

            migrationBuilder.RenameTable(
                name: "TiposEspecies",
                newName: "Categorias");
        }
    }
}
