using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _52_ListaMatanzaNumeradorPorEspecie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_NumeroLista",
                table: "ListasMatanzas");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_EspecieId_NumeroLista",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "EspecieId", "NumeroLista" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            // La generacion del NumeroLista pasa del MAX+1 al Numerador LISTAMATANZA: hay que dejar
            // cada numerador en el ultimo numero ya emitido, si no la proxima lista arrancaria en 1
            // y chocaria contra el indice unico. Se cuentan tambien las anuladas: no se reutilizan.
            migrationBuilder.Sql(@"
UPDATE n
SET n.UltimoNumero = x.MaxNumero, n.FechaActualizacion = SYSDATETIME()
FROM Numeradores n
INNER JOIN (
    SELECT EstablecimientoId, EspecieId, MAX(NumeroLista) AS MaxNumero
    FROM ListasMatanzas WHERE FechaBaja IS NULL
    GROUP BY EstablecimientoId, EspecieId
) x ON x.EstablecimientoId = n.EstablecimientoId AND x.EspecieId = n.EspecieCodigo
WHERE n.TipoNumerador = 'LISTAMATANZA' AND n.FechaBaja IS NULL AND n.UltimoNumero < x.MaxNumero;");

            // Combinaciones (establecimiento, especie) que ya tienen listas pero no tienen numerador.
            migrationBuilder.Sql(@"
INSERT INTO Numeradores (Id, EstablecimientoId, EspecieCodigo, Codigo, Descripcion, TipoNumerador, UltimoNumero, Activo, FechaActualizacion)
SELECT NEWID(), x.EstablecimientoId, x.EspecieId, 'LISTAMATANZA', 'Lista de Matanza', 'LISTAMATANZA', x.MaxNumero, 1, SYSDATETIME()
FROM (
    SELECT EstablecimientoId, EspecieId, MAX(NumeroLista) AS MaxNumero
    FROM ListasMatanzas WHERE FechaBaja IS NULL
    GROUP BY EstablecimientoId, EspecieId
) x
WHERE NOT EXISTS (
    SELECT 1 FROM Numeradores n
    WHERE n.EstablecimientoId = x.EstablecimientoId AND n.EspecieCodigo = x.EspecieId
      AND n.TipoNumerador = 'LISTAMATANZA' AND n.FechaBaja IS NULL);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_EspecieId_NumeroLista",
                table: "ListasMatanzas");

            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EstablecimientoId_NumeroLista",
                table: "ListasMatanzas",
                columns: new[] { "EstablecimientoId", "NumeroLista" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
        }
    }
}
