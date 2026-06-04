using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _15_FixEspeciesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Elimina EspecieId de Especies si existe (columna huerfana no gestionada por EF)
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Especies' AND COLUMN_NAME = 'EspecieId'
                )
                BEGIN
                    -- Eliminar FK si existe
                    DECLARE @fk NVARCHAR(256)
                    SELECT @fk = CONSTRAINT_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                    WHERE TABLE_NAME = 'Especies' AND COLUMN_NAME = 'EspecieId'
                    IF @fk IS NOT NULL
                        EXEC('ALTER TABLE Especies DROP CONSTRAINT ' + @fk)

                    -- Eliminar indice si existe
                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Especies') AND name = 'IX_Especies_EspecieId')
                        DROP INDEX IX_Especies_EspecieId ON Especies

                    ALTER TABLE Especies DROP COLUMN EspecieId
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
