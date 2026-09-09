using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <summary>
    /// Separa al dueno de la plataforma del administrador de una empresa.
    ///
    /// El padron de Empresas pasa a ser de SUPERADMIN: un ADMIN administra su empresa, pero
    /// no da de alta ni de baja empresas. Como sin ningun usuario con ese rol el padron
    /// quedaria inalcanzable, se crea tambien el usuario superadmin.
    ///
    /// Ese usuario pertenece a la primera empresa por dos razones concretas del modelo:
    /// Usuario.EmpresaId es obligatorio, y el login exige al menos una sucursal asignada.
    /// Administra el padron, pero opera dentro de esa empresa como cualquier otro usuario.
    /// </summary>
    public partial class _61_RolSuperadmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Roles WHERE Codigo = 'SUPERADMIN')
                    INSERT INTO Roles (Codigo, Nombre, Activo)
                    VALUES ('SUPERADMIN', 'SUPER ADMINISTRADOR', 1);");

            // SHA1 de 'superadmin', el mismo esquema que el resto de las contrasenas.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE UserName = 'superadmin')
                BEGIN
                    DECLARE @EmpresaId nvarchar(20) = (SELECT TOP 1 Id FROM Empresas ORDER BY Id);
                    DECLARE @SucursalId uniqueidentifier =
                        (SELECT TOP 1 Id FROM Sucursales WHERE EmpresaId = @EmpresaId AND FechaBaja IS NULL ORDER BY CodigoSucursal);

                    IF @EmpresaId IS NOT NULL AND @SucursalId IS NOT NULL
                    BEGIN
                        DECLARE @UsuarioId uniqueidentifier = NEWID();

                        INSERT INTO Usuarios (Id, UserName, PasswordHash, Nombre, Apellido, RolId, EmpresaId, FechaActualizacion, Activo)
                        VALUES (@UsuarioId, 'superadmin', '889A3A791B3875CFAE413574B53DA4BB8A90D53E',
                                'Super', 'Administrador', 'SUPERADMIN', @EmpresaId, GETDATE(), 1);

                        INSERT INTO UsuariosSucursales (Id, UsuarioId, SucursalId, EmpresaId, EsMain, FechaActualizacion)
                        VALUES (NEWID(), @UsuarioId, @SucursalId, @EmpresaId, 1, GETDATE());
                    END
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM UsuariosSucursales
                WHERE UsuarioId IN (SELECT Id FROM Usuarios WHERE UserName = 'superadmin');");
            migrationBuilder.Sql(@"DELETE FROM Usuarios WHERE UserName = 'superadmin';");
            migrationBuilder.Sql(@"DELETE FROM Roles WHERE Codigo = 'SUPERADMIN';");
        }
    }
}
