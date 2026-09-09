using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _63_TipoEspecieCatalogoGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // TiposEspecies pasa de tabla de empresa (PK Guid + EmpresaId, una copia identica
            // por empresa) a catalogo global (PK Codigo), y lo que cada empresa ajusta queda en
            // EmpresasTiposEspecies. Antes de aplanar el catalogo hay que guardarse el estado
            // viejo: es el mapa Guid -> Codigo que necesitan las cinco FKs, y es de donde sale
            // la configuracion de cada empresa.
            migrationBuilder.Sql(@"
                SELECT Id, Codigo, Nombre, EspecieId, TipoSexoId, ERP_Codigo, PesoTeorico,
                       Activo, FechaActualizacion, EmpresaId, FechaBaja
                INTO dbo.__MigracionTipoEspecie
                FROM dbo.TiposEspecies;");

            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX___MigracionTipoEspecie_Id
                    ON dbo.__MigracionTipoEspecie (Id);");

            // Misma deuda que las FK: la migracion 59 dejo declarados en el modelo los
            // indices de TipoEspecieId pero no los creo. El AlterColumn de mas abajo los va a
            // soltar y recrear, y falla si no estan, asi que se reponen antes.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_IngresosHaciendasPesadas_TipoEspecieId' AND object_id = OBJECT_ID('dbo.IngresosHaciendasPesadas'))
                    CREATE INDEX IX_IngresosHaciendasPesadas_TipoEspecieId ON dbo.IngresosHaciendasPesadas (TipoEspecieId);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_IngresosHaciendasUbicaciones_TipoEspecieId' AND object_id = OBJECT_ID('dbo.IngresosHaciendasUbicaciones'))
                    CREATE INDEX IX_IngresosHaciendasUbicaciones_TipoEspecieId ON dbo.IngresosHaciendasUbicaciones (TipoEspecieId);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ListasMatanzasDetalles_TipoEspecieId' AND object_id = OBJECT_ID('dbo.ListasMatanzasDetalles'))
                    CREATE INDEX IX_ListasMatanzasDetalles_TipoEspecieId ON dbo.ListasMatanzasDetalles (TipoEspecieId);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimientosCamaras_TipoEspecieId' AND object_id = OBJECT_ID('dbo.MovimientosCamaras'))
                    CREATE INDEX IX_MovimientosCamaras_TipoEspecieId ON dbo.MovimientosCamaras (TipoEspecieId);

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Tipificaciones_TipoEspecieId' AND object_id = OBJECT_ID('dbo.Tipificaciones'))
                    CREATE INDEX IX_Tipificaciones_TipoEspecieId ON dbo.Tipificaciones (TipoEspecieId);
");

            // Las FK hacia TiposEspecies se sueltan por SQL condicional y no con
            // DropForeignKey: la migracion 59 las dejo declaradas en el modelo pero no las
            // recreo en la base, asi que un DropForeignKey a secas falla donde nunca se
            // llegaron a crear.
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_IngresosHaciendasPesadas_TiposEspecies_TipoEspecieId')
                    ALTER TABLE dbo.IngresosHaciendasPesadas DROP CONSTRAINT FK_IngresosHaciendasPesadas_TiposEspecies_TipoEspecieId;");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_IngresosHaciendasUbicaciones_TiposEspecies_TipoEspecieId')
                    ALTER TABLE dbo.IngresosHaciendasUbicaciones DROP CONSTRAINT FK_IngresosHaciendasUbicaciones_TiposEspecies_TipoEspecieId;");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId')
                    ALTER TABLE dbo.ListasMatanzasDetalles DROP CONSTRAINT FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId;");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_MovimientosCamaras_TiposEspecies_TipoEspecieId')
                    ALTER TABLE dbo.MovimientosCamaras DROP CONSTRAINT FK_MovimientosCamaras_TiposEspecies_TipoEspecieId;");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Tipificaciones_TiposEspecies_TipoEspecieId')
                    ALTER TABLE dbo.Tipificaciones DROP CONSTRAINT FK_Tipificaciones_TiposEspecies_TipoEspecieId;");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposEspecies_Empresas_EmpresaId",
                table: "TiposEspecies");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposEspecies_Especies_EspecieId",
                table: "TiposEspecies");

            migrationBuilder.DropForeignKey(
                name: "FK_TiposEspecies_TiposSexos_TipoSexoId",
                table: "TiposEspecies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposEspecies",
                table: "TiposEspecies");

            migrationBuilder.DropIndex(
                name: "IX_TiposEspecies_EmpresaId_Codigo",
                table: "TiposEspecies");

            // Se queda una fila por Codigo. Gana la que no este dada de baja; entre iguales,
            // cualquiera: los datos de identidad de la categoria son los mismos en todas las
            // empresas, y los que difieren (peso, ERP) ya viajaron a __MigracionTipoEspecie.
            migrationBuilder.Sql(@"
                WITH Ranking AS (
                    SELECT Id,
                           ROW_NUMBER() OVER (
                               PARTITION BY Codigo
                               ORDER BY CASE WHEN FechaBaja IS NULL THEN 0 ELSE 1 END, EmpresaId
                           ) AS Orden
                    FROM dbo.TiposEspecies
                )
                DELETE FROM dbo.TiposEspecies
                WHERE Id IN (SELECT Id FROM Ranking WHERE Orden > 1);");

            // La fila que sobrevive representa a la categoria en si, no a la copia de una
            // empresa: si esa copia venia dada de baja, el catalogo igual la tiene que listar.
            migrationBuilder.Sql(@"
                UPDATE dbo.TiposEspecies SET FechaBaja = NULL WHERE FechaBaja IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TiposEspecies");

            migrationBuilder.DropColumn(
                name: "ERP_Codigo",
                table: "TiposEspecies");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "TiposEspecies");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "TiposEspecies");

            migrationBuilder.RenameColumn(
                name: "PesoTeorico",
                table: "TiposEspecies",
                newName: "PesoTeoricoReferencia");

            migrationBuilder.AlterColumn<string>(
                name: "Codigo",
                table: "TiposEspecies",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoEspecieId",
                table: "Tipificaciones",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoEspecieId",
                table: "MovimientosCamaras",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoEspecieId",
                table: "ListasMatanzasDetalles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "TipoEspecieId",
                table: "IngresosHaciendasUbicaciones",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TipoEspecieId",
                table: "IngresosHaciendasPesadas",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposEspecies",
                table: "TiposEspecies",
                column: "Codigo");

            // Las cinco FKs ya son nvarchar, pero traen el Guid escrito como texto: el
            // AlterColumn convierte el tipo, no el significado. Aca se cambia cada Guid por el
            // Codigo al que apuntaba, que es la clave del catalogo nuevo.
            migrationBuilder.Sql(@"
                UPDATE d
                SET d.TipoEspecieId = m.Codigo
                FROM dbo.Tipificaciones d
                JOIN dbo.__MigracionTipoEspecie m
                    ON CONVERT(nvarchar(450), m.Id) = d.TipoEspecieId;

                UPDATE d
                SET d.TipoEspecieId = m.Codigo
                FROM dbo.MovimientosCamaras d
                JOIN dbo.__MigracionTipoEspecie m
                    ON CONVERT(nvarchar(450), m.Id) = d.TipoEspecieId;

                UPDATE d
                SET d.TipoEspecieId = m.Codigo
                FROM dbo.ListasMatanzasDetalles d
                JOIN dbo.__MigracionTipoEspecie m
                    ON CONVERT(nvarchar(450), m.Id) = d.TipoEspecieId;

                UPDATE d
                SET d.TipoEspecieId = m.Codigo
                FROM dbo.IngresosHaciendasUbicaciones d
                JOIN dbo.__MigracionTipoEspecie m
                    ON CONVERT(nvarchar(450), m.Id) = d.TipoEspecieId;

                UPDATE d
                SET d.TipoEspecieId = m.Codigo
                FROM dbo.IngresosHaciendasPesadas d
                JOIN dbo.__MigracionTipoEspecie m
                    ON CONVERT(nvarchar(450), m.Id) = d.TipoEspecieId;");

            // Valores del nomenclador del rubro, los mismos con los que nace una empresa
            // (empresa-base.json). El peso de aca es de referencia: no lo usa ningun calculo,
            // es lo que se propone cuando una empresa da de alta la categoria. Si una empresa
            // habia editado el nombre, gana el del nomenclador.
            migrationBuilder.Sql(@"
                DECLARE @Base TABLE (
                    Codigo nvarchar(450), Nombre nvarchar(max), EspecieId nvarchar(450),
                    TipoSexoId nvarchar(450), PesoTeoricoReferencia float);

                INSERT INTO @Base (Codigo, Nombre, EspecieId, TipoSexoId, PesoTeoricoReferencia)
                VALUES  ('CA', 'CAPONES / HEMBRAS SIN SERVICIO', 'P', 'M', 90),
                        ('CH', 'CHANCHAS', 'P', 'H', 100),
                        ('CO', 'CACHORRO', 'P', 'M', 85),
                        ('LE', 'LECHONES', 'P', 'M', 120),
                        ('ME', 'MACHO ENTERO INMUNOLOGICAMENTE CASTRADO', 'P', 'M', 110),
                        ('PA', 'PADRILLOS', 'P', 'M', 100),
                        ('MJ', 'MEJ', 'V', 'M', 90),
                        ('NO', 'NOVILLO', 'V', 'M', 90),
                        ('NT', 'NOVILLITO', 'V', 'M', 85),
                        ('TO', 'TORO', 'V', 'M', 94),
                        ('VA', 'VACA', 'V', 'H', 92),
                        ('VQ', 'VAQUILLONA', 'V', 'H', 98);

                UPDATE te
                SET te.Nombre = b.Nombre,
                    te.EspecieId = b.EspecieId,
                    te.TipoSexoId = b.TipoSexoId,
                    te.PesoTeoricoReferencia = b.PesoTeoricoReferencia,
                    te.Activo = 1
                FROM dbo.TiposEspecies te
                JOIN @Base b ON b.Codigo = te.Codigo;

                INSERT INTO dbo.TiposEspecies
                    (Codigo, Nombre, EspecieId, TipoSexoId, PesoTeoricoReferencia, Activo, FechaBaja)
                SELECT b.Codigo, b.Nombre, b.EspecieId, b.TipoSexoId, b.PesoTeoricoReferencia, 1, NULL
                FROM @Base b
                WHERE NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = b.Codigo);");

            // Ninguna fila de operacion puede quedar apuntando a un codigo que no exista: si
            // pasa, la FK de mas abajo falla igual, pero con un error que no dice donde.
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM dbo.Tipificaciones d
                    WHERE d.TipoEspecieId IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = d.TipoEspecieId)
                )
                    THROW 50000, 'Quedaron TipoEspecieId sin mapear en Tipificaciones.', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.MovimientosCamaras d
                    WHERE d.TipoEspecieId IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = d.TipoEspecieId)
                )
                    THROW 50000, 'Quedaron TipoEspecieId sin mapear en MovimientosCamaras.', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.ListasMatanzasDetalles d
                    WHERE d.TipoEspecieId IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = d.TipoEspecieId)
                )
                    THROW 50000, 'Quedaron TipoEspecieId sin mapear en ListasMatanzasDetalles.', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.IngresosHaciendasUbicaciones d
                    WHERE d.TipoEspecieId IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = d.TipoEspecieId)
                )
                    THROW 50000, 'Quedaron TipoEspecieId sin mapear en IngresosHaciendasUbicaciones.', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.IngresosHaciendasPesadas d
                    WHERE d.TipoEspecieId IS NOT NULL
                      AND NOT EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = d.TipoEspecieId)
                )
                    THROW 50000, 'Quedaron TipoEspecieId sin mapear en IngresosHaciendasPesadas.', 1;");

            migrationBuilder.CreateTable(
                name: "EmpresasTiposEspecies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoEspecieId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PesoTeorico = table.Column<double>(type: "float", nullable: false),
                    ERP_Codigo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpresaId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasTiposEspecies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresasTiposEspecies_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresasTiposEspecies_TiposEspecies_TipoEspecieId",
                        column: x => x.TipoEspecieId,
                        principalTable: "TiposEspecies",
                        principalColumn: "Codigo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasTiposEspecies_EmpresaId_TipoEspecieId",
                table: "EmpresasTiposEspecies",
                columns: new[] { "EmpresaId", "TipoEspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasTiposEspecies_TipoEspecieId",
                table: "EmpresasTiposEspecies",
                column: "TipoEspecieId");

            // Cada fila vieja era la copia que una empresa tenia del catalogo: ahi estaban
            // su peso teorico, su codigo de ERP y si operaba o no con la categoria. Eso es
            // exactamente la configuracion por empresa, asi que se muda tal cual y nadie
            // pierde lo que habia ajustado.
            migrationBuilder.Sql(@"
                INSERT INTO dbo.EmpresasTiposEspecies
                    (Id, TipoEspecieId, PesoTeorico, ERP_Codigo, Activo, FechaActualizacion, EmpresaId, FechaBaja)
                SELECT NEWID(), m.Codigo, m.PesoTeorico, m.ERP_Codigo, m.Activo,
                       m.FechaActualizacion, m.EmpresaId, m.FechaBaja
                FROM dbo.__MigracionTipoEspecie m
                WHERE EXISTS (SELECT 1 FROM dbo.TiposEspecies te WHERE te.Codigo = m.Codigo)
                  AND m.Id = (
                      SELECT TOP 1 m2.Id
                      FROM dbo.__MigracionTipoEspecie m2
                      WHERE m2.EmpresaId = m.EmpresaId AND m2.Codigo = m.Codigo
                      ORDER BY CASE WHEN m2.FechaBaja IS NULL THEN 0 ELSE 1 END, m2.Id
                  );");

            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendasPesadas_TiposEspecies_TipoEspecieId",
                table: "IngresosHaciendasPesadas",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendasUbicaciones_TiposEspecies_TipoEspecieId",
                table: "IngresosHaciendasUbicaciones",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzasDetalles_TiposEspecies_TipoEspecieId",
                table: "ListasMatanzasDetalles",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCamaras_TiposEspecies_TipoEspecieId",
                table: "MovimientosCamaras",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_TiposEspecies_TipoEspecieId",
                table: "Tipificaciones",
                column: "TipoEspecieId",
                principalTable: "TiposEspecies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposEspecies_Especies_EspecieId",
                table: "TiposEspecies",
                column: "EspecieId",
                principalTable: "Especies",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TiposEspecies_TiposSexos_TipoSexoId",
                table: "TiposEspecies",
                column: "TipoSexoId",
                principalTable: "TiposSexos",
                principalColumn: "Codigo",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"DROP TABLE dbo.__MigracionTipoEspecie;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "El pase de TiposEspecies a catalogo global no es reversible: funde la copia "
                + "que cada empresa tenia del catalogo en una sola fila y remapea las claves "
                + "de cinco tablas. Para volver atras, restaurar el backup previo.");
        }
    }
}
