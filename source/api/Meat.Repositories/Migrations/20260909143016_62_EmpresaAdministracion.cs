using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <summary>
    /// El SUPERADMIN deja de vivir en una empresa que opera.
    ///
    /// La migracion 61 lo puso en la primera empresa y le asigno una de sus sucursales, porque
    /// el login exigia tener una. Eso dejaba al administrador de la plataforma adentro de un
    /// frigorifico concreto, y ademas cruzaba tenants: un usuario de una empresa apuntando a la
    /// sucursal de otra.
    ///
    /// Ahora existe la empresa ADM (tipo AD, administrativa): sin sucursales ni establecimientos,
    /// solo para administrar el padron y la configuracion general. El login ya no pide sucursal
    /// cuando la empresa no tiene ninguna, asi que el vinculo cruzado se puede soltar.
    /// </summary>
    public partial class _62_EmpresaAdministracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM TiposEmpresas WHERE Codigo = 'AD')
                    INSERT INTO TiposEmpresas (Codigo, Nombre, Activo)
                    VALUES ('AD', 'ADMINISTRACION', 1);");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Empresas WHERE Id = 'ADM')
                    INSERT INTO Empresas (Id, Nombre, TipoEmpresaId, Activo, FechaActualizacion)
                    VALUES ('ADM', 'Administracion', 'AD', 1, GETDATE());");

            // El superadmin se muda a ADM y suelta la sucursal de la otra empresa.
            migrationBuilder.Sql(@"
                DELETE us
                FROM UsuariosSucursales us
                JOIN Usuarios u ON u.Id = us.UsuarioId
                WHERE u.UserName = 'superadmin';");

            migrationBuilder.Sql(@"
                UPDATE Usuarios SET EmpresaId = 'ADM' WHERE UserName = 'superadmin';");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Se devuelve el superadmin a la primera empresa, con una de sus sucursales:
            // es la unica forma de que pueda entrar con el login anterior.
            migrationBuilder.Sql(@"
                DECLARE @EmpresaId nvarchar(20) = (SELECT TOP 1 Id FROM Empresas WHERE Id <> 'ADM' ORDER BY Id);
                DECLARE @SucursalId uniqueidentifier =
                    (SELECT TOP 1 Id FROM Sucursales WHERE EmpresaId = @EmpresaId AND FechaBaja IS NULL ORDER BY CodigoSucursal);
                DECLARE @UsuarioId uniqueidentifier = (SELECT Id FROM Usuarios WHERE UserName = 'superadmin');

                IF @UsuarioId IS NOT NULL AND @EmpresaId IS NOT NULL AND @SucursalId IS NOT NULL
                BEGIN
                    UPDATE Usuarios SET EmpresaId = @EmpresaId WHERE Id = @UsuarioId;

                    IF NOT EXISTS (SELECT 1 FROM UsuariosSucursales WHERE UsuarioId = @UsuarioId)
                        INSERT INTO UsuariosSucursales (Id, UsuarioId, SucursalId, EmpresaId, EsMain, FechaActualizacion)
                        VALUES (NEWID(), @UsuarioId, @SucursalId, @EmpresaId, 1, GETDATE());
                END

                DELETE FROM Empresas WHERE Id = 'ADM';");
        }
    }
}
