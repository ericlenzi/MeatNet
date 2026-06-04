using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _16_RemoveEmpresaIdFromEspecies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'Especies' AND COLUMN_NAME = 'EmpresaId'
                )
                BEGIN
                    DECLARE @fk NVARCHAR(256)
                    SELECT @fk = fk.name
                    FROM sys.foreign_keys fk
                    JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                    JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
                    WHERE fk.parent_object_id = OBJECT_ID('Especies') AND c.name = 'EmpresaId'

                    IF @fk IS NOT NULL
                        EXEC('ALTER TABLE Especies DROP CONSTRAINT ' + @fk)

                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Especies') AND name = 'IX_Especies_EmpresaId')
                        DROP INDEX IX_Especies_EmpresaId ON Especies

                    ALTER TABLE Especies DROP COLUMN EmpresaId
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
