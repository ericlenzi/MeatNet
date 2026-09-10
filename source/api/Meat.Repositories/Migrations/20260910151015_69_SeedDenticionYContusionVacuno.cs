using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _69_SeedDenticionYContusionVacuno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Denticion y contusion para vacuno: los otros dos datos que el tipificador registra
            // en el palco, junto con conformacion y engrasamiento (migracion 67). Solo vacuno,
            // por la misma razon que los otros dos: la res porcina no se clasifica con escalas
            // visuales sino por porcentaje de carne magra (R-E21).
            //
            // Denticion = recuento de incisivos permanentes, que estima la edad del animal.
            // Orden es la posicion en la escala, de menor a mayor edad.
            //
            // Solo se insertan los codigos que falten: si alguien ya los cargo a mano desde la
            // pantalla, la migracion no los pisa ni falla.
            migrationBuilder.Sql(@"
                DECLARE @Denticion TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int);
                INSERT INTO @Denticion (Codigo, Nombre, Orden)
                VALUES  ('D0', 'Diente de leche',  0),
                        ('D2', 'Dos dientes',      1),
                        ('D4', 'Cuatro dientes',   2),
                        ('D6', 'Seis dientes',     3),
                        ('D8', 'Boca llena',       4);

                INSERT INTO dbo.Denticiones (Codigo, Nombre, EspecieId, Orden, Activo, FechaBaja)
                SELECT d.Codigo, d.Nombre, 'V', d.Orden, 1, NULL
                FROM @Denticion d
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'V')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Denticiones x WHERE x.Codigo = d.Codigo);");

            // Contusion = golpe visible en la media res, por eso se registra por pieza y no por
            // animal. 'SC' (sin contusion) no es relleno: es lo que hace que el dato sea
            // obligatorio de verdad, porque el tipificador tiene que pronunciarse tambien cuando
            // la res esta sana. Orden va de menor a mayor severidad, y el primero de la escala es
            // el que el Tipificador propone por defecto.
            migrationBuilder.Sql(@"
                DECLARE @Contusion TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int);
                INSERT INTO @Contusion (Codigo, Nombre, Orden)
                VALUES  ('SC',  'Sin contusion', 0),
                        ('LEV', 'Leve',          1),
                        ('MOD', 'Moderada',      2),
                        ('GRA', 'Grave',         3);

                INSERT INTO dbo.TiposContusiones (Codigo, Nombre, EspecieId, Orden, Activo, FechaBaja)
                SELECT c.Codigo, c.Nombre, 'V', c.Orden, 1, NULL
                FROM @Contusion c
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'V')
                  AND NOT EXISTS (SELECT 1 FROM dbo.TiposContusiones x WHERE x.Codigo = c.Codigo);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Se borran solo si nadie los uso todavia: un romaneo registrado no puede quedar
            // apuntando a un codigo que ya no existe.
            migrationBuilder.Sql(@"
                DELETE FROM dbo.Denticiones
                WHERE EspecieId = 'V' AND Codigo IN ('D0','D2','D4','D6','D8')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Romaneos r WHERE r.DenticionId = Codigo);

                DELETE FROM dbo.TiposContusiones
                WHERE EspecieId = 'V' AND Codigo IN ('SC','LEV','MOD','GRA')
                  AND NOT EXISTS (SELECT 1 FROM dbo.RomaneosPiezas p WHERE p.TipoContusionId = Codigo);");
        }
    }
}
