using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _75_SeedMotivosDecomisoPorcino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Motivos de decomiso para porcino. Habilita los decomisos en las jornadas de cerdos:
            // hasta ahora el catalogo solo tenia filas de vacuno, y por R-E25 la especie sin
            // motivos activos no puede registrar decomisos (el Tipificador ni muestra el check).
            //
            // OJO CON LOS CODIGOS. La PK de MotivosDecomisos es el Codigo solo, no (Codigo,
            // Especie), asi que 'CONT' ya esta tomado por la contusion de vacuno y no se puede
            // repetir. Por eso los de porcino van con prefijo 'P-'. No es cosmetico: sin prefijo
            // la migracion falla con violacion de PK.
            //
            // La lista mezcla las causas comunes con las tipicas del cerdo (neumonia, serositis,
            // artritis, erisipela, dermatitis), que en playa porcina pesan mas que en vacuna.
            // Sigue siendo un juego inicial, no el nomenclador oficial: el SUPERADMIN lo ajusta
            // desde la pantalla. El Orden es la posicion en la lista del puesto, las frecuentes
            // primero.
            //
            // Solo se insertan los codigos que falten: si alguien ya los cargo a mano desde la
            // pantalla, la migracion no los pisa ni falla.
            migrationBuilder.Sql(@"
                DECLARE @Motivo TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int, ExigeContusion bit);
                INSERT INTO @Motivo (Codigo, Nombre, Orden, ExigeContusion)
                VALUES  ('P-CONT',   'Contusiones',                1, 1),
                        ('P-ABS',    'Abscesos',                   2, 0),
                        ('P-ADH',    'Adherencias',                3, 0),
                        ('P-PERI',   'Pericarditis y pleuritis',   4, 0),
                        ('P-NEUM',   'Neumonia',                   5, 0),
                        ('P-ART',    'Artritis',                   6, 0),
                        ('P-CONTAM', 'Contaminacion',              7, 0),
                        ('P-MSAN',   'Mala sangria',               8, 0),
                        ('P-ERIS',   'Erisipela',                  9, 0),
                        ('P-DERM',   'Dermatitis y sarna',        10, 0),
                        ('P-ICT',    'Ictericia',                 11, 0),
                        ('P-CAQ',    'Caquexia',                  12, 0),
                        ('P-CIST',   'Cisticercosis',             13, 0),
                        ('P-TBC',    'Tuberculosis',              14, 0),
                        ('P-SEPT',   'Septicemia',                15, 0),
                        ('P-OTR',    'Otros',                     99, 0);

                INSERT INTO dbo.MotivosDecomisos (Codigo, Nombre, EspecieId, Orden, ExigeContusion, Activo, FechaBaja)
                SELECT m.Codigo, m.Nombre, 'P', m.Orden, m.ExigeContusion, 1, NULL
                FROM @Motivo m
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'P')
                  AND NOT EXISTS (SELECT 1 FROM dbo.MotivosDecomisos x WHERE x.Codigo = m.Codigo);");

            // 'P-CONT' queda marcado como motivo de golpe (R-E26), igual que su par de vacuno.
            // Hoy la marca no tiene efecto porque porcino no tiene escala de contusiones cargada
            // y la regla se saltea sola; el dia que se carguen, empieza a exigirla sin tocar nada.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Se borran solo los que nadie uso todavia: un decomiso registrado no puede quedar
            // apuntando a un codigo que ya no existe.
            migrationBuilder.Sql(@"
                DELETE FROM dbo.MotivosDecomisos
                WHERE EspecieId = 'P' AND Codigo LIKE 'P-%'
                  AND NOT EXISTS (SELECT 1 FROM dbo.Romaneos r WHERE r.MotivoDecomisoId = Codigo)
                  AND NOT EXISTS (SELECT 1 FROM dbo.RomaneosPiezas p WHERE p.MotivoDecomisoId = Codigo);");
        }
    }
}
