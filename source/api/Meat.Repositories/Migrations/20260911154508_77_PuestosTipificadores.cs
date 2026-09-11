using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _77_PuestosTipificadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RomaneosPiezasMediciones_TiposMediciones_TipoMedicionId",
                table: "RomaneosPiezasMediciones");

            migrationBuilder.DropIndex(
                name: "IX_Puestos_EmpresaId",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "Erp_Codigo",
                table: "Puestos");

            migrationBuilder.RenameColumn(
                name: "TipoMedicionId",
                table: "RomaneosPiezasMediciones",
                newName: "TipoMagnitudId");

            migrationBuilder.RenameIndex(
                name: "IX_RomaneosPiezasMediciones_TipoMedicionId",
                table: "RomaneosPiezasMediciones",
                newName: "IX_RomaneosPiezasMediciones_TipoMagnitudId");

            migrationBuilder.AddColumn<Guid>(
                name: "PuestoId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TipificadorId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMedicionId",
                table: "Romaneos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodigoPuesto",
                table: "Puestos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EspecieId",
                table: "Puestos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoMedicionId",
                table: "Puestos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tipificadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Matricula = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EstablecimientoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EspecieId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PorDefecto = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpresaId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipificadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Especies_EspecieId",
                        column: x => x.EspecieId,
                        principalTable: "Especies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tipificadores_Establecimientos_EstablecimientoId",
                        column: x => x.EstablecimientoId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposMagnitudes",
                columns: table => new
                {
                    Codigo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMagnitudes", x => x.Codigo);
                });

            // --- Datos: TiposMediciones cambia de significado ---------------------------------
            // La tabla guardaba QUE se mide (PESO). Ahora eso es TiposMagnitudes, y
            // TiposMediciones pasa a guardar COMO se mide (manual, balanza, automatica).
            // Las filas viejas se mudan a la tabla nueva, que es donde apunta el romaneo.
            migrationBuilder.Sql(@"
                INSERT INTO TiposMagnitudes (Codigo, Nombre, Activo, FechaBaja)
                SELECT Codigo, Nombre, Activo, FechaBaja FROM TiposMediciones;

                DELETE FROM TiposMediciones;

                INSERT INTO TiposMediciones (Codigo, Nombre, Activo) VALUES
                    ('M', 'Manual', 1),
                    ('B', 'Balanza', 1),
                    ('A', 'Automatica', 1);
            ");

            // --- Datos: TiposPuestos vuelve a ser una clasificacion ---------------------------
            // Tenia una fila por especie (PALCO-V, PALCO-P), que es informacion del puesto y no
            // de su clase. Queda una sola clase, PAL, y la especie pasa a ser columna del Puesto.
            // Las filas viejas se dan de baja logica; ya no quedan puestos apuntandoles.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM TiposPuestos WHERE Codigo = 'PAL')
                    INSERT INTO TiposPuestos (Codigo, Nombre, Activo) VALUES ('PAL', 'Palco', 1);

                UPDATE Puestos SET EspecieId = 'V' WHERE EspecieId IS NULL AND TipoPuestoId = 'PALCO-V';
                UPDATE Puestos SET EspecieId = 'P' WHERE EspecieId IS NULL AND TipoPuestoId = 'PALCO-P';

                -- Un puesto de otra clase se queda con la unica especie de su establecimiento,
                -- si tiene una sola; si opera con varias, lo define el usuario en la pantalla.
                UPDATE p SET EspecieId = (
                        SELECT MIN(ee.EspecieId) FROM EstablecimientosEspecies ee
                        WHERE ee.EstablecimientoId = p.EstablecimientoId AND ee.FechaBaja IS NULL
                        HAVING COUNT(DISTINCT ee.EspecieId) = 1)
                    FROM Puestos p WHERE p.EspecieId IS NULL;

                UPDATE Puestos SET TipoPuestoId = 'PAL' WHERE TipoPuestoId LIKE 'PALCO%';

                -- El metodo de medicion arranca en manual: es como se venia cargando el peso.
                UPDATE Puestos SET TipoMedicionId = 'M' WHERE TipoMedicionId IS NULL;

                UPDATE TiposPuestos SET FechaBaja = SYSDATETIME()
                WHERE Codigo IN ('PALCO-V', 'PALCO-P') AND FechaBaja IS NULL;
            ");

            // --- Datos: la cabecera del puesto en lo ya cargado -------------------------------
            // La lista de matanza ahora declara su palco. Las listas que venian sin puesto toman
            // el unico puesto activo de su establecimiento y especie, si hay uno solo: asi las
            // jornadas ya cargadas siguen siendo visibles desde el palco. Los romaneos copian
            // el puesto de su lista y quedan como medicion manual, que es lo que se hizo.
            migrationBuilder.Sql(@"
                UPDATE lm SET PuestoId = (
                        SELECT MIN(pu.Id) FROM Puestos pu
                        WHERE pu.EstablecimientoId = lm.EstablecimientoId
                            AND pu.EspecieId = lm.EspecieId
                            AND pu.Activo = 1
                            AND pu.FechaBaja IS NULL
                        HAVING COUNT(*) = 1)
                    FROM ListasMatanzas lm WHERE lm.PuestoId IS NULL;

                UPDATE r SET PuestoId = lm.PuestoId
                    FROM Romaneos r
                    INNER JOIN ListasMatanzas lm ON lm.Id = r.ListaMatanzaId
                    WHERE r.PuestoId IS NULL;

                UPDATE Romaneos SET TipoMedicionId = 'M' WHERE TipoMedicionId IS NULL;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_PuestoId",
                table: "Romaneos",
                column: "PuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TipificadorId",
                table: "Romaneos",
                column: "TipificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_TipoMedicionId",
                table: "Romaneos",
                column: "TipoMedicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EmpresaId_CodigoPuesto",
                table: "Puestos",
                columns: new[] { "EmpresaId", "CodigoPuesto" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EspecieId",
                table: "Puestos",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_TipoMedicionId",
                table: "Puestos",
                column: "TipoMedicionId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EmpresaId_Matricula",
                table: "Tipificadores",
                columns: new[] { "EmpresaId", "Matricula" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EspecieId",
                table: "Tipificadores",
                column: "EspecieId");

            migrationBuilder.CreateIndex(
                name: "IX_Tipificadores_EstablecimientoId_EspecieId",
                table: "Tipificadores",
                columns: new[] { "EstablecimientoId", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [PorDefecto] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_Especies_EspecieId",
                table: "Puestos",
                column: "EspecieId",
                principalTable: "Especies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_TiposMediciones_TipoMedicionId",
                table: "Puestos",
                column: "TipoMedicionId",
                principalTable: "TiposMediciones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Puestos_PuestoId",
                table: "Romaneos",
                column: "PuestoId",
                principalTable: "Puestos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Tipificadores_TipificadorId",
                table: "Romaneos",
                column: "TipificadorId",
                principalTable: "Tipificadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_TiposMediciones_TipoMedicionId",
                table: "Romaneos",
                column: "TipoMedicionId",
                principalTable: "TiposMediciones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezasMediciones_TiposMagnitudes_TipoMagnitudId",
                table: "RomaneosPiezasMediciones",
                column: "TipoMagnitudId",
                principalTable: "TiposMagnitudes",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Puestos_Especies_EspecieId",
                table: "Puestos");

            migrationBuilder.DropForeignKey(
                name: "FK_Puestos_TiposMediciones_TipoMedicionId",
                table: "Puestos");

            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Puestos_PuestoId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_Tipificadores_TipificadorId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_TiposMediciones_TipoMedicionId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_RomaneosPiezasMediciones_TiposMagnitudes_TipoMagnitudId",
                table: "RomaneosPiezasMediciones");

            // TiposMediciones vuelve a ser el catalogo de magnitudes: se devuelven sus filas
            // antes de tirar la tabla nueva.
            migrationBuilder.Sql(@"
                DELETE FROM TiposMediciones;

                INSERT INTO TiposMediciones (Codigo, Nombre, Activo, FechaBaja)
                SELECT Codigo, Nombre, Activo, FechaBaja FROM TiposMagnitudes;

                UPDATE TiposPuestos SET FechaBaja = NULL WHERE Codigo IN ('PALCO-V', 'PALCO-P');
            ");

            migrationBuilder.DropTable(
                name: "Tipificadores");

            migrationBuilder.DropTable(
                name: "TiposMagnitudes");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_PuestoId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_TipificadorId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Romaneos_TipoMedicionId",
                table: "Romaneos");

            migrationBuilder.DropIndex(
                name: "IX_Puestos_EmpresaId_CodigoPuesto",
                table: "Puestos");

            migrationBuilder.DropIndex(
                name: "IX_Puestos_EspecieId",
                table: "Puestos");

            migrationBuilder.DropIndex(
                name: "IX_Puestos_TipoMedicionId",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "PuestoId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "TipificadorId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "TipoMedicionId",
                table: "Romaneos");

            migrationBuilder.DropColumn(
                name: "EspecieId",
                table: "Puestos");

            migrationBuilder.DropColumn(
                name: "TipoMedicionId",
                table: "Puestos");

            migrationBuilder.RenameColumn(
                name: "TipoMagnitudId",
                table: "RomaneosPiezasMediciones",
                newName: "TipoMedicionId");

            migrationBuilder.RenameIndex(
                name: "IX_RomaneosPiezasMediciones_TipoMagnitudId",
                table: "RomaneosPiezasMediciones",
                newName: "IX_RomaneosPiezasMediciones_TipoMedicionId");

            migrationBuilder.AlterColumn<string>(
                name: "CodigoPuesto",
                table: "Puestos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Erp_Codigo",
                table: "Puestos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EmpresaId",
                table: "Puestos",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezasMediciones_TiposMediciones_TipoMedicionId",
                table: "RomaneosPiezasMediciones",
                column: "TipoMedicionId",
                principalTable: "TiposMediciones",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
