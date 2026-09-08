using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <summary>
    /// MeatNet pasa a ser multiempresa.
    ///
    /// El Up() esta escrito en SQL a mano, no con las operaciones que genero el scaffolding:
    /// EF resolvia el cambio de tipo de las claves con DROP + ADD de columna, lo que borraria
    /// los datos, y ademas emparejaba CodigoEmpresa con Logo generando un rename equivocado.
    /// Aca cada cambio de clave es agregar la columna nueva, copiar el dato, verificar que no
    /// quedaron huerfanos y recien entonces soltar la vieja.
    ///
    /// Tres cosas pasan, en este orden:
    ///  1. Empresa deja de tener PK Guid: su clave primaria es el codigo de negocio
    ///     (el ex CodigoEmpresa). Suma Color y Logo.
    ///  2. Toda tabla propia de una empresa recibe EmpresaId y hereda la empresa de quien cuelga.
    ///  3. Las cinco tablas que pasaron a ser propias de una empresa cambian su PK de string
    ///     a Guid; el string baja a columna Codigo, unica dentro de la empresa.
    ///
    /// No es reversible: para volver atras hay que restaurar el backup previo.
    /// </summary>
    public partial class _59_Multiempresa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Suelta todo lo que ata a una columna (FK, default, indices) para poder bajarla.
            // Se resuelve por catalogo y no por nombre: los default constraints que dejaron
            // las migraciones viejas tienen nombres autogenerados, distintos en cada base.
            string SoltarDependencias(string tabla, string columna) => $@"
                DECLARE @s nvarchar(max) = N'';

                SELECT @s = @s + N'ALTER TABLE {tabla} DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';'
                FROM sys.foreign_keys fk
                JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
                JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
                WHERE fk.parent_object_id = OBJECT_ID('{tabla}') AND c.name = '{columna}';

                SELECT @s = @s + N'ALTER TABLE {tabla} DROP CONSTRAINT ' + QUOTENAME(dc.name) + N';'
                FROM sys.default_constraints dc
                JOIN sys.columns c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
                WHERE dc.parent_object_id = OBJECT_ID('{tabla}') AND c.name = '{columna}';

                SELECT @s = @s + CASE WHEN i.is_unique_constraint = 1
                                      THEN N'ALTER TABLE {tabla} DROP CONSTRAINT ' + QUOTENAME(i.name) + N';'
                                      ELSE N'DROP INDEX ' + QUOTENAME(i.name) + N' ON {tabla};' END
                FROM sys.indexes i
                JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
                JOIN sys.columns c ON c.object_id = i.object_id AND c.column_id = ic.column_id
                WHERE i.object_id = OBJECT_ID('{tabla}') AND c.name = '{columna}' AND i.is_primary_key = 0;

                EXEC sp_executesql @s;";
            // ============================================================
            // 1. Empresa: la clave primaria pasa a ser el codigo de negocio
            // ============================================================

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM Empresas WHERE CodigoEmpresa IS NULL OR LTRIM(RTRIM(CodigoEmpresa)) = '')
                    THROW 50001, 'Hay empresas sin CodigoEmpresa: no puede ser la clave primaria.', 1;
                IF EXISTS (SELECT CodigoEmpresa FROM Empresas GROUP BY CodigoEmpresa HAVING COUNT(*) > 1)
                    THROW 50002, 'Hay CodigoEmpresa duplicados: no puede ser la clave primaria.', 1;
                IF EXISTS (SELECT 1 FROM Empresas WHERE LEN(CodigoEmpresa) > 20)
                    THROW 50003, 'Hay CodigoEmpresa de mas de 20 caracteres.', 1;");

            migrationBuilder.Sql(@"
                ALTER TABLE Empresas ADD IdNuevo nvarchar(20) NULL, Color nvarchar(max) NULL, Logo nvarchar(max) NULL;");
            migrationBuilder.Sql(@"UPDATE Empresas SET IdNuevo = CodigoEmpresa;");

            // Las tres tablas que ya apuntaban a Empresa: su FK pasa de Guid al codigo.
            foreach (var tabla in new[] { "Sucursales", "Establecimientos", "Parametros" })
            {
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ADD EmpresaIdNuevo nvarchar(20) NULL;");
                migrationBuilder.Sql($@"
                    UPDATE t SET t.EmpresaIdNuevo = e.CodigoEmpresa
                    FROM {tabla} t JOIN Empresas e ON e.Id = t.EmpresaId;");
                migrationBuilder.Sql($@"
                    IF EXISTS (SELECT 1 FROM {tabla} WHERE EmpresaIdNuevo IS NULL)
                        THROW 50010, 'Quedaron filas de {tabla} sin empresa al convertir la clave.', 1;");
            }

            // Se sueltan FK, indices y PK que cuelgan de la clave vieja.
            migrationBuilder.Sql(@"
                DECLARE @s nvarchar(max) = N'';
                SELECT @s = @s + N'ALTER TABLE ' + QUOTENAME(OBJECT_NAME(parent_object_id))
                                + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';'
                FROM sys.foreign_keys WHERE referenced_object_id = OBJECT_ID('Empresas');
                EXEC sp_executesql @s;");

            foreach (var tabla in new[] { "Sucursales", "Establecimientos", "Parametros" })
                migrationBuilder.Sql(SoltarDependencias(tabla, "EmpresaId"));

            migrationBuilder.Sql(@"
                DECLARE @s nvarchar(max);
                SELECT @s = N'ALTER TABLE Empresas DROP CONSTRAINT ' + QUOTENAME(name) + N';'
                FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('Empresas') AND type = 'PK';
                EXEC sp_executesql @s;");

            migrationBuilder.Sql(SoltarDependencias("Empresas", "Id"));
            migrationBuilder.Sql(SoltarDependencias("Empresas", "CodigoEmpresa"));
            migrationBuilder.Sql(@"ALTER TABLE Empresas DROP COLUMN Id;");
            migrationBuilder.Sql(@"ALTER TABLE Empresas DROP COLUMN CodigoEmpresa;");

            // Resto de una migracion vieja: la tabla Empresas tenia una columna EmpresaId que
            // nunca se mapeo y quedo con datos sin sentido. Ahora que EmpresaId significa algo
            // en todas las demas tablas, dejarla seria confuso.
            // La columna arrastraba un indice propio (la FK auto-referencial ya cayo mas
            // arriba, junto con el resto de las que apuntaban a Empresas).
            migrationBuilder.Sql(SoltarDependencias("Empresas", "EmpresaId"));
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Empresas') AND name = 'EmpresaId')
                    ALTER TABLE Empresas DROP COLUMN EmpresaId;");
            migrationBuilder.Sql(@"EXEC sp_rename 'Empresas.IdNuevo', 'Id', 'COLUMN';");
            // En comandos separados a proposito: SQL Server compila el batch completo y
            // rechazaria la PK por estar la columna todavia declarada nullable.
            migrationBuilder.Sql(@"ALTER TABLE Empresas ALTER COLUMN Id nvarchar(20) NOT NULL;");
            migrationBuilder.Sql(@"ALTER TABLE Empresas ADD CONSTRAINT PK_Empresas PRIMARY KEY (Id);");

            foreach (var tabla in new[] { "Sucursales", "Establecimientos", "Parametros" })
            {
                migrationBuilder.Sql(SoltarDependencias(tabla, "EmpresaId"));
                migrationBuilder.Sql($@"ALTER TABLE {tabla} DROP COLUMN EmpresaId;");
                migrationBuilder.Sql($@"EXEC sp_rename '{tabla}.EmpresaIdNuevo', 'EmpresaId', 'COLUMN';");
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ALTER COLUMN EmpresaId nvarchar(20) NOT NULL;");
            }

            // ============================================================
            // 2. EmpresaId en toda tabla propia de una empresa
            // ============================================================

            // Cada tabla hereda la empresa de aquella de la que cuelga; el orden va de padres a hijos.
            var derivadas = new (string Tabla, string Join)[]
            {
                ("EstablecimientosEspecies",     "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("ClientesEstablecimientos",     "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("UsuariosEstablecimientos",     "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("UsuariosSucursales",           "JOIN Sucursales p ON p.Id = t.SucursalId"),
                ("Almacenes",                    "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("Puestos",                      "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("Numeradores",                  "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("IngresosHaciendas",            "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("ListasMatanzas",               "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("Romaneos",                     "JOIN Establecimientos p ON p.Id = t.EstablecimientoId"),
                ("IngresosHaciendasPesadas",     "JOIN IngresosHaciendas p ON p.Id = t.IngresoHaciendaId"),
                ("IngresosHaciendasUbicaciones", "JOIN IngresosHaciendas p ON p.Id = t.IngresoHaciendaId"),
                ("Tropas",                       "JOIN IngresosHaciendas p ON p.Id = t.IngresoHaciendaId"),
                ("ListasMatanzasDetalles",       "JOIN ListasMatanzas p ON p.Id = t.ListaMatanzaId"),
                ("ListasMatanzasMovimientos",    "JOIN ListasMatanzas p ON p.Id = t.ListaMatanzaId"),
                ("RomaneosPiezas",               "JOIN Romaneos p ON p.Id = t.RomaneoId"),
                ("MovimientosCamaras",           "JOIN Almacenes p ON p.Id = t.AlmacenId"),
                ("NumeradoresTropas",            "JOIN ClientesEstablecimientos p ON p.Id = t.ClienteEstablecimientoId"),
                ("TropasMovimientos",            "JOIN Tropas p ON p.Id = t.TropaId"),
                ("RomaneosPiezasMediciones",     "JOIN RomaneosPiezas p ON p.Id = t.RomaneoPiezaId"),
                ("DespiecesMateriales",          "JOIN Materiales p ON p.Id = t.MaterialOrigenId"),
            };

            // Estas eran comunes a todas las empresas: no hay camino a una empresa, asi que
            // pasan a la que ya opera. Es la unica lectura posible al partir de un sistema
            // de una sola empresa.
            var exGlobales = new[]
            {
                "Usuarios", "Clientes", "Materiales", "UnidadesFaenas", "TiposEspecies", "DestinosComerciales",
            };

            var propiasDeEmpresa = new System.Collections.Generic.List<string>();
            propiasDeEmpresa.AddRange(System.Linq.Enumerable.Select(derivadas, d => d.Tabla));
            propiasDeEmpresa.AddRange(exGlobales);
            propiasDeEmpresa.Add("Tipificaciones");

            foreach (var tabla in propiasDeEmpresa)
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ADD EmpresaId nvarchar(20) NULL;");

            migrationBuilder.Sql($@"
                DECLARE @EmpresaDefault nvarchar(20) = (
                    SELECT TOP 1 e.Id FROM Empresas e
                    LEFT JOIN Establecimientos es ON es.EmpresaId = e.Id
                    GROUP BY e.Id ORDER BY COUNT(es.Id) DESC, e.Id);
                IF @EmpresaDefault IS NULL THROW 50011, 'No hay ninguna empresa cargada.', 1;
                {string.Join("\n                ", System.Linq.Enumerable.Select(exGlobales,
                    t => $"UPDATE {t} SET EmpresaId = @EmpresaDefault;"))}");

            // Tipificacion ya traia el codigo de empresa, como columna suelta sin FK.
            migrationBuilder.Sql(@"UPDATE Tipificaciones SET EmpresaId = CodigoEmpresa;");

            foreach (var d in derivadas)
                migrationBuilder.Sql($@"UPDATE t SET t.EmpresaId = p.EmpresaId FROM {d.Tabla} t {d.Join};");

            foreach (var tabla in propiasDeEmpresa)
                migrationBuilder.Sql($@"
                    IF EXISTS (SELECT 1 FROM {tabla} WHERE EmpresaId IS NULL)
                        THROW 50012, 'Quedaron filas sin empresa en {tabla}.', 1;");

            foreach (var tabla in propiasDeEmpresa)
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ALTER COLUMN EmpresaId nvarchar(20) NOT NULL;");

            migrationBuilder.Sql(SoltarDependencias("Tipificaciones", "CodigoEmpresa"));
            migrationBuilder.Sql(@"ALTER TABLE Tipificaciones DROP COLUMN CodigoEmpresa;");

            // ============================================================
            // 3. Las cinco tablas que pasan de PK string a Guid
            // ============================================================
            // El string no se pierde: baja a columna Codigo, unica dentro de la empresa.
            // Es el paso que puede romper datos en silencio, asi que cada FK remapeada se
            // verifica antes de soltar la columna vieja.

            var conPkNueva = new[] { "TiposEspecies", "UnidadesFaenas", "DestinosComerciales", "Tipificaciones", "Parametros" };

            // a) soltar las FK que apuntan a estas tablas y sus PK
            foreach (var tabla in conPkNueva)
            {
                migrationBuilder.Sql($@"
                    DECLARE @s nvarchar(max) = N'';
                    SELECT @s = @s + N'ALTER TABLE ' + QUOTENAME(OBJECT_NAME(parent_object_id))
                                    + N' DROP CONSTRAINT ' + QUOTENAME(name) + N';'
                    FROM sys.foreign_keys WHERE referenced_object_id = OBJECT_ID('{tabla}');
                    EXEC sp_executesql @s;");
                migrationBuilder.Sql($@"
                    DECLARE @s nvarchar(max);
                    SELECT @s = N'ALTER TABLE {tabla} DROP CONSTRAINT ' + QUOTENAME(name) + N';'
                    FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('{tabla}') AND type = 'PK';
                    EXEC sp_executesql @s;");
            }

            // b) en TiposEspecies el string se llamaba Id: pasa a llamarse Codigo
            migrationBuilder.Sql(@"EXEC sp_rename 'TiposEspecies.Id', 'Codigo', 'COLUMN';");

            // c) clave nueva
            foreach (var tabla in conPkNueva)
            {
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ADD Id uniqueidentifier NULL;");
                migrationBuilder.Sql($@"UPDATE {tabla} SET Id = NEWID();");
            }

            // d) remapeo de las nueve FK: del codigo al Guid, dentro de la misma empresa
            var remapeos = new (string Principal, string Hija, string Columna, bool Obligatoria)[]
            {
                ("TiposEspecies",      "Tipificaciones",               "TipoEspecieId",      false),
                ("TiposEspecies",      "IngresosHaciendasPesadas",     "TipoEspecieId",      false),
                ("TiposEspecies",      "IngresosHaciendasUbicaciones", "TipoEspecieId",      false),
                ("TiposEspecies",      "ListasMatanzasDetalles",       "TipoEspecieId",      true),
                ("TiposEspecies",      "MovimientosCamaras",           "TipoEspecieId",      false),
                ("UnidadesFaenas",     "Tipificaciones",               "UnidadFaenaId",      true),
                ("UnidadesFaenas",     "Romaneos",                     "UnidadFaenaId",      true),
                ("DestinosComerciales","Tipificaciones",               "DestinoComercialId", false),
                ("Tipificaciones",     "RomaneosPiezas",               "TipificacionId",     false),
            };

            foreach (var r in remapeos)
            {
                migrationBuilder.Sql($@"ALTER TABLE {r.Hija} ADD {r.Columna}Nuevo uniqueidentifier NULL;");
                migrationBuilder.Sql($@"
                    UPDATE t SET t.{r.Columna}Nuevo = p.Id
                    FROM {r.Hija} t
                    JOIN {r.Principal} p ON p.Codigo = t.{r.Columna} AND p.EmpresaId = t.EmpresaId;");
                // Si un codigo no resolvio, la fila quedaria desconectada en silencio: se aborta.
                migrationBuilder.Sql($@"
                    IF EXISTS (SELECT 1 FROM {r.Hija} WHERE {r.Columna} IS NOT NULL AND {r.Columna}Nuevo IS NULL)
                        THROW 50020, 'Hay filas de {r.Hija}.{r.Columna} que no resuelven contra {r.Principal}.', 1;");
                migrationBuilder.Sql(SoltarDependencias(r.Hija, r.Columna));
                migrationBuilder.Sql($@"ALTER TABLE {r.Hija} DROP COLUMN {r.Columna};");
                migrationBuilder.Sql($@"EXEC sp_rename '{r.Hija}.{r.Columna}Nuevo', '{r.Columna}', 'COLUMN';");
                if (r.Obligatoria)
                    migrationBuilder.Sql($@"ALTER TABLE {r.Hija} ALTER COLUMN {r.Columna} uniqueidentifier NOT NULL;");
            }

            // e) la clave nueva queda firme y el codigo pasa a ser un atributo mas
            foreach (var tabla in conPkNueva)
            {
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ALTER COLUMN Id uniqueidentifier NOT NULL;");
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ADD CONSTRAINT PK_{tabla} PRIMARY KEY (Id);");
                migrationBuilder.Sql($@"ALTER TABLE {tabla} ALTER COLUMN Codigo nvarchar(450) NULL;");
            }

            // ============================================================
            // 4. Indices y claves foraneas
            // ============================================================
            // Los codigos de negocio dejan de ser unicos globalmente y pasan a serlo
            // dentro de cada empresa. Algunos indices ya cayeron al soltar sus columnas,
            // por eso el drop es condicional.

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UnidadesFaenas_EspecieId' AND object_id = OBJECT_ID('UnidadesFaenas'))
                    DROP INDEX IX_UnidadesFaenas_EspecieId ON UnidadesFaenas;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sucursales_CodigoSucursal' AND object_id = OBJECT_ID('Sucursales'))
                    DROP INDEX IX_Sucursales_CodigoSucursal ON Sucursales;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Sucursales_EmpresaId' AND object_id = OBJECT_ID('Sucursales'))
                    DROP INDEX IX_Sucursales_EmpresaId ON Sucursales;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Parametros_EmpresaId' AND object_id = OBJECT_ID('Parametros'))
                    DROP INDEX IX_Parametros_EmpresaId ON Parametros;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Materiales_CodigoMaterial' AND object_id = OBJECT_ID('Materiales'))
                    DROP INDEX IX_Materiales_CodigoMaterial ON Materiales;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Establecimientos_CodigoEstablecimiento' AND object_id = OBJECT_ID('Establecimientos'))
                    DROP INDEX IX_Establecimientos_CodigoEstablecimiento ON Establecimientos;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Establecimientos_EmpresaId' AND object_id = OBJECT_ID('Establecimientos'))
                    DROP INDEX IX_Establecimientos_EmpresaId ON Establecimientos;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DestinosComerciales_Favorito' AND object_id = OBJECT_ID('DestinosComerciales'))
                    DROP INDEX IX_DestinosComerciales_Favorito ON DestinosComerciales;
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Clientes_CodigoCliente' AND object_id = OBJECT_ID('Clientes'))
                    DROP INDEX IX_Clientes_CodigoCliente ON Clientes;");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosSucursales_EmpresaId",
                table: "UsuariosSucursales",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_UsuariosEstablecimientos_EmpresaId",
                table: "UsuariosEstablecimientos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EmpresaId_Codigo",
                table: "UnidadesFaenas",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EmpresaId_EspecieId",
                table: "UnidadesFaenas",
                columns: new[] { "EmpresaId", "EspecieId" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [PorDefecto] = 1");
            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EspecieId",
                table: "UnidadesFaenas",
                column: "EspecieId");
            migrationBuilder.CreateIndex(
                name: "IX_TropasMovimientos_EmpresaId",
                table: "TropasMovimientos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Tropas_EmpresaId",
                table: "Tropas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_TiposEspecies_EmpresaId_Codigo",
                table: "TiposEspecies",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_Tipificaciones_EmpresaId_Codigo",
                table: "Tipificaciones",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_Sucursales_EmpresaId_CodigoSucursal",
                table: "Sucursales",
                columns: new[] { "EmpresaId", "CodigoSucursal" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezasMediciones_EmpresaId",
                table: "RomaneosPiezasMediciones",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_RomaneosPiezas_EmpresaId",
                table: "RomaneosPiezas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Romaneos_EmpresaId",
                table: "Romaneos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Puestos_EmpresaId",
                table: "Puestos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Parametros_EmpresaId_Codigo",
                table: "Parametros",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_NumeradoresTropas_EmpresaId",
                table: "NumeradoresTropas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Numeradores_EmpresaId",
                table: "Numeradores",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCamaras_EmpresaId",
                table: "MovimientosCamaras",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Materiales_EmpresaId_CodigoMaterial",
                table: "Materiales",
                columns: new[] { "EmpresaId", "CodigoMaterial" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasMovimientos_EmpresaId",
                table: "ListasMatanzasMovimientos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzasDetalles_EmpresaId",
                table: "ListasMatanzasDetalles",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_ListasMatanzas_EmpresaId",
                table: "ListasMatanzas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasUbicaciones_EmpresaId",
                table: "IngresosHaciendasUbicaciones",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendasPesadas_EmpresaId",
                table: "IngresosHaciendasPesadas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_IngresosHaciendas_EmpresaId",
                table: "IngresosHaciendas",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_EstablecimientosEspecies_EmpresaId",
                table: "EstablecimientosEspecies",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_EmpresaId_CodigoEstablecimiento",
                table: "Establecimientos",
                columns: new[] { "EmpresaId", "CodigoEstablecimiento" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_DestinosComerciales_EmpresaId_Codigo",
                table: "DestinosComerciales",
                columns: new[] { "EmpresaId", "Codigo" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_DestinosComerciales_EmpresaId_Favorito",
                table: "DestinosComerciales",
                columns: new[] { "EmpresaId", "Favorito" },
                unique: true,
                filter: "[FechaBaja] IS NULL AND [Favorito] = 1");
            migrationBuilder.CreateIndex(
                name: "IX_DespiecesMateriales_EmpresaId",
                table: "DespiecesMateriales",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_ClientesEstablecimientos_EmpresaId",
                table: "ClientesEstablecimientos",
                column: "EmpresaId");
            migrationBuilder.CreateIndex(
                name: "IX_Clientes_EmpresaId_CodigoCliente",
                table: "Clientes",
                columns: new[] { "EmpresaId", "CodigoCliente" },
                unique: true,
                filter: "[FechaBaja] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_EmpresaId",
                table: "Almacenes",
                column: "EmpresaId");
            migrationBuilder.AddForeignKey(
                name: "FK_Almacenes_Empresas_EmpresaId",
                table: "Almacenes",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Empresas_EmpresaId",
                table: "Clientes",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ClientesEstablecimientos_Empresas_EmpresaId",
                table: "ClientesEstablecimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DespiecesMateriales_Empresas_EmpresaId",
                table: "DespiecesMateriales",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_DestinosComerciales_Empresas_EmpresaId",
                table: "DestinosComerciales",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Establecimientos_Empresas_EmpresaId",
                table: "Establecimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_EstablecimientosEspecies_Empresas_EmpresaId",
                table: "EstablecimientosEspecies",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendas_Empresas_EmpresaId",
                table: "IngresosHaciendas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendasPesadas_Empresas_EmpresaId",
                table: "IngresosHaciendasPesadas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IngresosHaciendasUbicaciones_Empresas_EmpresaId",
                table: "IngresosHaciendasUbicaciones",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzas_Empresas_EmpresaId",
                table: "ListasMatanzas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzasDetalles_Empresas_EmpresaId",
                table: "ListasMatanzasDetalles",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_ListasMatanzasMovimientos_Empresas_EmpresaId",
                table: "ListasMatanzasMovimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Materiales_Empresas_EmpresaId",
                table: "Materiales",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosCamaras_Empresas_EmpresaId",
                table: "MovimientosCamaras",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Numeradores_Empresas_EmpresaId",
                table: "Numeradores",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_NumeradoresTropas_Empresas_EmpresaId",
                table: "NumeradoresTropas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Parametros_Empresas_EmpresaId",
                table: "Parametros",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Puestos_Empresas_EmpresaId",
                table: "Puestos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_Empresas_EmpresaId",
                table: "Romaneos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_UnidadesFaenas_UnidadFaenaId",
                table: "Romaneos",
                column: "UnidadFaenaId",
                principalTable: "UnidadesFaenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezas_Empresas_EmpresaId",
                table: "RomaneosPiezas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezas_Tipificaciones_TipificacionId",
                table: "RomaneosPiezas",
                column: "TipificacionId",
                principalTable: "Tipificaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_RomaneosPiezasMediciones_Empresas_EmpresaId",
                table: "RomaneosPiezasMediciones",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Sucursales_Empresas_EmpresaId",
                table: "Sucursales",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_DestinosComerciales_DestinoComercialId",
                table: "Tipificaciones",
                column: "DestinoComercialId",
                principalTable: "DestinosComerciales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_Empresas_EmpresaId",
                table: "Tipificaciones",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId",
                table: "Tipificaciones",
                column: "UnidadFaenaId",
                principalTable: "UnidadesFaenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TiposEspecies_Empresas_EmpresaId",
                table: "TiposEspecies",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Tropas_Empresas_EmpresaId",
                table: "Tropas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_TropasMovimientos_Empresas_EmpresaId",
                table: "TropasMovimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UnidadesFaenas_Empresas_EmpresaId",
                table: "UnidadesFaenas",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosEstablecimientos_Empresas_EmpresaId",
                table: "UsuariosEstablecimientos",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_UsuariosSucursales_Empresas_EmpresaId",
                table: "UsuariosSucursales",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException(
                "La migracion a multiempresa no es reversible: convierte claves primarias y "
                + "reparte los datos entre empresas. Para volver atras, restaurar el backup previo.");
        }
    }
}
