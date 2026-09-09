using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _67_SeedEjesTipificacionVacuno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ejes de la tipificacion oficial para vacuno. La migracion 66 creo las tablas vacias
            // a proposito; estos son los valores que aporto el usuario.
            //
            // Orden = posicion en la escala, no ranking de calidad. En conformacion coinciden
            // (A es la mejor y E la peor), pero en engrasamiento NO: el optimo es el 2, y tanto el
            // 0 como el 4 son extremos indeseados. Por eso Orden guarda 0,1,2,3,4 tal cual.
            //
            // Solo se insertan los codigos que falten: si alguien ya los cargo a mano desde la
            // pantalla, la migracion no los pisa ni falla.
            migrationBuilder.Sql(@"
                DECLARE @Conformacion TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int);
                INSERT INTO @Conformacion (Codigo, Nombre, Orden)
                VALUES  ('A', 'Superior',    1),
                        ('B', 'Buena',       2),
                        ('C', 'Intermedia',  3),
                        ('D', 'Deficiente',  4),
                        ('E', 'Inferior',    5);

                INSERT INTO dbo.Conformaciones (Codigo, Nombre, EspecieId, Orden, Activo, FechaBaja)
                SELECT c.Codigo, c.Nombre, 'V', c.Orden, 1, NULL
                FROM @Conformacion c
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'V')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Conformaciones x WHERE x.Codigo = c.Codigo);");

            migrationBuilder.Sql(@"
                DECLARE @Engrasamiento TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int);
                INSERT INTO @Engrasamiento (Codigo, Nombre, Orden)
                VALUES  ('0', 'Sin grasa',  0),
                        ('1', 'Escaso',     1),
                        ('2', 'Adecuado',   2),
                        ('3', 'Abundante',  3),
                        ('4', 'Excesivo',   4);

                INSERT INTO dbo.GradosEngrasamiento (Codigo, Nombre, EspecieId, Orden, Activo, FechaBaja)
                SELECT g.Codigo, g.Nombre, 'V', g.Orden, 1, NULL
                FROM @Engrasamiento g
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'V')
                  AND NOT EXISTS (SELECT 1 FROM dbo.GradosEngrasamiento x WHERE x.Codigo = g.Codigo);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Se borran solo si nadie los uso todavia: un romaneo tipificado no puede quedar
            // apuntando a un codigo que ya no existe.
            migrationBuilder.Sql(@"
                DELETE FROM dbo.Conformaciones
                WHERE EspecieId = 'V' AND Codigo IN ('A','B','C','D','E')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Romaneos r WHERE r.ConformacionId = Codigo);

                DELETE FROM dbo.GradosEngrasamiento
                WHERE EspecieId = 'V' AND Codigo IN ('0','1','2','3','4')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Romaneos r WHERE r.GradoEngrasamientoId = Codigo);");
        }
    }
}
