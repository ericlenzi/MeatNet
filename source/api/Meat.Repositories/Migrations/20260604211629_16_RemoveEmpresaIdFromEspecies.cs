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
                    DECLARE @sql NVARCHAR(MAX) = ''

                    -- Eliminar DEFAULT constraints
                    SELECT @sql = @sql + 'ALTER TABLE Especies DROP CONSTRAINT ' + dc.name + '; '
                    FROM sys.default_constraints dc
                    JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
                    WHERE dc.parent_object_id = OBJECT_ID('Especies') AND c.name = 'EmpresaId'

                    -- Eliminar FK constraints
                    SELECT @sql = @sql + 'ALTER TABLE Especies DROP CONSTRAINT ' + fk.name + '; '
                    FROM sys.foreign_keys fk
                    JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                    JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
                    WHERE fk.parent_object_id = OBJECT_ID('Especies') AND c.name = 'EmpresaId'

                    -- Eliminar indice si existe
                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('Especies') AND name = 'IX_Especies_EmpresaId')
                        SET @sql = @sql + 'DROP INDEX IX_Especies_EmpresaId ON Especies; '

                    EXEC sp_executesql @sql

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
