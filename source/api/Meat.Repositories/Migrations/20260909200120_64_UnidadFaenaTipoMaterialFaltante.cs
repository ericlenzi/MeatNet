using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _64_UnidadFaenaTipoMaterialFaltante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La columna TipoMaterialId de UnidadFaena la agrego la migracion 55 y quedo vacia en
            // las filas que ya existian. Es el eje de forma que alinea la unidad con el catalogo
            // de Materiales, y la validacion de la Tipificacion (que el material sea de la misma
            // forma que la unidad) esta escrita como "si la unidad tiene TipoMaterial": con la
            // columna vacia el chequeo no corre y una media res puede terminar tipificada contra
            // un material de cuarto sin que nadie se entere.
            //
            // Se completa por el codigo de la unidad, usando el mismo mapeo con el que nace una
            // empresa (empresa-base.json). Solo se tocan las filas vacias, y solo los codigos
            // conocidos: si una empresa creo unidades propias, quedan sin completar a proposito y
            // las tiene que resolver su ADMIN, que es el unico que sabe que forma tienen.
            migrationBuilder.Sql(@"
                DECLARE @Base TABLE (Codigo nvarchar(450), TipoMaterialId nvarchar(450));

                INSERT INTO @Base (Codigo, TipoMaterialId)
                VALUES  ('1P', 'RES'),        ('2P', 'MEDIA_RES'), ('3P', 'CUARTO'),
                        ('4P', 'CUARTO'),     ('5P', 'CUARTO'),    ('6P', 'CUARTO'),
                        ('7P', 'CUARTO'),     ('8P', 'CUARTO'),    ('9P', 'RES'),
                        ('1V', 'RES'),        ('2V', 'MEDIA_RES'), ('3V', 'CUARTO'),
                        ('4V', 'CUARTO'),     ('9V', 'MEDIA_RES');

                UPDATE uf
                SET uf.TipoMaterialId = b.TipoMaterialId
                FROM dbo.UnidadesFaenas uf
                JOIN @Base b ON b.Codigo = uf.Codigo
                WHERE uf.TipoMaterialId IS NULL
                  AND EXISTS (SELECT 1 FROM dbo.TiposMateriales tm WHERE tm.Codigo = b.TipoMaterialId);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Vaciar la columna de nuevo dejaria el modelo peor de lo que estaba y no hay forma de
            // distinguir las filas que completo esta migracion de las que ya venian cargadas.
            // Es una correccion de datos: no se deshace.
        }
    }
}
