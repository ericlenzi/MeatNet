using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _71_SeedMotivosDecomisoVacuno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Motivos de decomiso para vacuno: la causa sanitaria por la que la inspeccion
            // condena una res entera (R-E23) o retira kilos de una media res (R-E24).
            //
            // Es un juego inicial de causas frecuentes en playa, no el nomenclador oficial
            // completo: la lista definitiva la ajusta el SUPERADMIN desde la pantalla, cargando
            // o desactivando filas, sin tocar codigo ni desplegar nada.
            //
            // La misma fila sirve para los dos decomisos: la causa es la misma, lo que cambia es
            // el alcance. 'Contusiones' condena una res cuando es extensa y es un recorte cuando
            // esta localizada; por eso el motivo no lleva marcado si es total o parcial.
            //
            // Orden es la posicion en la lista del puesto: las causas mas frecuentes primero,
            // para que el tipificador no tenga que buscar. No es una escala de gravedad.
            //
            // Solo se insertan los codigos que falten: si alguien ya los cargo a mano desde la
            // pantalla, la migracion no los pisa ni falla.
            migrationBuilder.Sql(@"
                DECLARE @Motivo TABLE (Codigo nvarchar(450), Nombre nvarchar(max), Orden int);
                INSERT INTO @Motivo (Codigo, Nombre, Orden)
                VALUES  ('CONT',   'Contusiones',                  1),
                        ('ABS',    'Abscesos',                     2),
                        ('ADH',    'Adherencias',                  3),
                        ('CONTAM', 'Contaminacion',                4),
                        ('MSAN',   'Mala sangria',                 5),
                        ('ICT',    'Ictericia',                    6),
                        ('CAQ',    'Caquexia',                     7),
                        ('TBC',    'Tuberculosis',                 8),
                        ('CIST',   'Cisticercosis',                9),
                        ('HID',    'Hidatidosis',                 10),
                        ('SEPT',   'Septicemia',                  11),
                        ('OTR',    'Otros',                       99);

                INSERT INTO dbo.MotivosDecomisos (Codigo, Nombre, EspecieId, Orden, Activo, FechaBaja)
                SELECT m.Codigo, m.Nombre, 'V', m.Orden, 1, NULL
                FROM @Motivo m
                WHERE EXISTS (SELECT 1 FROM dbo.Especies e WHERE e.Codigo = 'V')
                  AND NOT EXISTS (SELECT 1 FROM dbo.MotivosDecomisos x WHERE x.Codigo = m.Codigo);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Se borran solo los que nadie uso todavia: un decomiso registrado no puede quedar
            // apuntando a un codigo que ya no existe. El motivo vive en dos lugares, la res
            // condenada y la media res recortada, asi que los dos bloquean el borrado.
            migrationBuilder.Sql(@"
                DELETE FROM dbo.MotivosDecomisos
                WHERE EspecieId = 'V'
                  AND Codigo IN ('CONT','ABS','ADH','CONTAM','MSAN','ICT','CAQ','TBC','CIST','HID','SEPT','OTR')
                  AND NOT EXISTS (SELECT 1 FROM dbo.Romaneos r WHERE r.MotivoDecomisoId = Codigo)
                  AND NOT EXISTS (SELECT 1 FROM dbo.RomaneosPiezas p WHERE p.MotivoDecomisoId = Codigo);");
        }
    }
}
