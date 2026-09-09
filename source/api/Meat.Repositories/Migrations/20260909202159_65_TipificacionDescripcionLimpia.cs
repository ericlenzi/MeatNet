using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _65_TipificacionDescripcionLimpia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Las descripciones se guardaban tal cual venian del formulario, sin recortar, y
            // varias quedaron con un salto de linea al final (se ven cortadas o con un renglon
            // vacio en las grillas). Los handlers de alta y edicion ahora hacen Trim; esto limpia
            // lo que ya estaba cargado.
            //
            // Los saltos se reemplazan por un espacio y recien despues se recorta: si alguno
            // quedo en el medio de la descripcion, las dos palabras no se pegan.
            migrationBuilder.Sql(@"
                UPDATE dbo.Tipificaciones
                SET Descripcion = LTRIM(RTRIM(
                        REPLACE(REPLACE(Descripcion, CHAR(13), ' '), CHAR(10), ' ')))
                WHERE Descripcion IS NOT NULL
                  AND Descripcion <> LTRIM(RTRIM(
                        REPLACE(REPLACE(Descripcion, CHAR(13), ' '), CHAR(10), ' ')));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reponer los saltos de linea no tendria sentido: eran basura de carga, no un dato.
        }
    }
}
