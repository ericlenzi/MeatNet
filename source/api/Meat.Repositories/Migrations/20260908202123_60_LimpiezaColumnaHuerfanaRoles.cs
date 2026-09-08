using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <summary>
    /// Roles.EmpresaId es una columna que quedo en la base desde 14_RemoveEmpresaIdFromGlobalEntities:
    /// nunca se mapeo en el modelo y guarda el Guid viejo de la empresa 1, un identificador que
    /// tras 59_Multiempresa ya no existe (la clave de Empresa es ahora su codigo).
    ///
    /// Rol es un catalogo comun a todas las empresas, asi que la columna no debe volver.
    /// Se baja junto con su default constraint, cuyo nombre es autogenerado.
    /// </summary>
    public partial class _60_LimpiezaColumnaHuerfanaRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @s nvarchar(max) = N'';

                SELECT @s = @s + N'ALTER TABLE Roles DROP CONSTRAINT ' + QUOTENAME(dc.name) + N';'
                FROM sys.default_constraints dc
                JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
                WHERE dc.parent_object_id = OBJECT_ID('Roles') AND c.name = 'EmpresaId';

                SELECT @s = @s + N'DROP INDEX ' + QUOTENAME(i.name) + N' ON Roles;'
                FROM sys.indexes i
                JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                JOIN sys.columns c ON c.object_id = i.object_id AND c.column_id = ic.column_id
                WHERE i.object_id = OBJECT_ID('Roles') AND c.name = 'EmpresaId' AND i.is_primary_key = 0;

                EXEC sp_executesql @s;");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Roles') AND name = 'EmpresaId')
                    ALTER TABLE Roles DROP COLUMN EmpresaId;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // La columna no tenia sentido en el modelo: no se repone.
        }
    }
}
