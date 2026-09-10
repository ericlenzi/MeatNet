using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _76_PrefijoVEnMotivosDecomisoVacuno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los motivos de vacuno pasan a llevar prefijo 'V-', para que el catalogo se lea igual
            // en las dos especies ('V-CONT' y 'P-CONT' en vez de 'CONT' y 'P-CONT'). El prefijo no
            // es cosmetico: la PK de MotivosDecomisos es el Codigo solo, asi que el codigo tiene
            // que decir de que especie es, y con las dos especies prefijadas la convencion queda
            // clara para el que sume ovinos.
            //
            // NO es un UPDATE del codigo. El Codigo es la PK y hay romaneos apuntandole, con FK
            // Restrict: renombrar en el lugar lo rechaza la base. La secuencia es insertar los
            // codigos nuevos, repuntar las dos referencias y recien despues borrar los viejos.
            migrationBuilder.Sql(@"
                DECLARE @Renombre TABLE (Viejo nvarchar(450), Nuevo nvarchar(450));
                INSERT INTO @Renombre (Viejo, Nuevo)
                SELECT Codigo, 'V-' + Codigo
                FROM dbo.MotivosDecomisos
                WHERE EspecieId = 'V' AND Codigo NOT LIKE 'V-%';

                -- 1) Los codigos nuevos, con todo lo que tenian los viejos.
                INSERT INTO dbo.MotivosDecomisos (Codigo, Nombre, EspecieId, Orden, ExigeContusion, Activo, FechaBaja)
                SELECT r.Nuevo, m.Nombre, m.EspecieId, m.Orden, m.ExigeContusion, m.Activo, m.FechaBaja
                FROM @Renombre r
                JOIN dbo.MotivosDecomisos m ON m.Codigo = r.Viejo
                WHERE NOT EXISTS (SELECT 1 FROM dbo.MotivosDecomisos x WHERE x.Codigo = r.Nuevo);

                -- 2) Las dos referencias: la res condenada y la media res (condenada o recortada).
                UPDATE ro SET MotivoDecomisoId = r.Nuevo
                FROM dbo.Romaneos ro
                JOIN @Renombre r ON r.Viejo = ro.MotivoDecomisoId;

                UPDATE p SET MotivoDecomisoId = r.Nuevo
                FROM dbo.RomaneosPiezas p
                JOIN @Renombre r ON r.Viejo = p.MotivoDecomisoId;

                -- 3) Recien ahora los viejos quedan sin nadie apuntandoles.
                DELETE m
                FROM dbo.MotivosDecomisos m
                JOIN @Renombre r ON r.Viejo = m.Codigo;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // La inversa, en el mismo orden: sin esto, la baja de la migracion 71 no encontraria
            // los codigos que ella misma sembro.
            migrationBuilder.Sql(@"
                DECLARE @Renombre TABLE (Viejo nvarchar(450), Nuevo nvarchar(450));
                INSERT INTO @Renombre (Viejo, Nuevo)
                SELECT Codigo, SUBSTRING(Codigo, 3, LEN(Codigo) - 2)
                FROM dbo.MotivosDecomisos
                WHERE EspecieId = 'V' AND Codigo LIKE 'V-%';

                INSERT INTO dbo.MotivosDecomisos (Codigo, Nombre, EspecieId, Orden, ExigeContusion, Activo, FechaBaja)
                SELECT r.Nuevo, m.Nombre, m.EspecieId, m.Orden, m.ExigeContusion, m.Activo, m.FechaBaja
                FROM @Renombre r
                JOIN dbo.MotivosDecomisos m ON m.Codigo = r.Viejo
                WHERE NOT EXISTS (SELECT 1 FROM dbo.MotivosDecomisos x WHERE x.Codigo = r.Nuevo);

                UPDATE ro SET MotivoDecomisoId = r.Nuevo
                FROM dbo.Romaneos ro
                JOIN @Renombre r ON r.Viejo = ro.MotivoDecomisoId;

                UPDATE p SET MotivoDecomisoId = r.Nuevo
                FROM dbo.RomaneosPiezas p
                JOIN @Renombre r ON r.Viejo = p.MotivoDecomisoId;

                DELETE m
                FROM dbo.MotivosDecomisos m
                JOIN @Renombre r ON r.Viejo = m.Codigo;");
        }
    }
}
