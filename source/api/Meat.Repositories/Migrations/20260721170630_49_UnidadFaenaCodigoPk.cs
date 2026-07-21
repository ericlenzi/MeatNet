using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _49_UnidadFaenaCodigoPk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cambio de PK de UnidadesFaenas: Guid Id -> string Codigo, y baja de Numero.
            // Se preservan los datos: se genera un Codigo secuencial ("1","2",...) por
            // (EspecieId, Numero) y se propaga a los FKs (Romaneos, Tipificaciones) antes
            // de eliminar las columnas viejas.

            // 1) Nueva columna Codigo (temporalmente nullable) y su poblado secuencial.
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas ADD Codigo nvarchar(450) NULL;");
            migrationBuilder.Sql(@"
                ;WITH cte AS (
                    SELECT Id, CAST(ROW_NUMBER() OVER (ORDER BY EspecieId, Numero) AS nvarchar(450)) AS rn
                    FROM UnidadesFaenas
                )
                UPDATE u SET u.Codigo = cte.rn
                FROM UnidadesFaenas u INNER JOIN cte ON u.Id = cte.Id;");

            // 2) Columnas temporales string en los FKs, pobladas por join sobre el Guid viejo.
            migrationBuilder.Sql("ALTER TABLE Tipificaciones ADD UnidadFaenaCodigo nvarchar(450) NULL;");
            migrationBuilder.Sql(@"
                UPDATE t SET t.UnidadFaenaCodigo = u.Codigo
                FROM Tipificaciones t INNER JOIN UnidadesFaenas u ON t.UnidadFaenaId = u.Id;");
            migrationBuilder.Sql("ALTER TABLE Romaneos ADD UnidadFaenaCodigo nvarchar(450) NULL;");
            migrationBuilder.Sql(@"
                UPDATE r SET r.UnidadFaenaCodigo = u.Codigo
                FROM Romaneos r INNER JOIN UnidadesFaenas u ON r.UnidadFaenaId = u.Id;");

            // 3) Bajar FKs e indices viejos de los FKs.
            migrationBuilder.Sql("ALTER TABLE Tipificaciones DROP CONSTRAINT FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId;");
            migrationBuilder.Sql("DROP INDEX IX_Tipificaciones_UnidadFaenaId ON Tipificaciones;");
            migrationBuilder.Sql("ALTER TABLE Romaneos DROP CONSTRAINT FK_Romaneos_UnidadesFaenas_UnidadFaenaId;");
            migrationBuilder.Sql("DROP INDEX IX_Romaneos_UnidadFaenaId ON Romaneos;");

            // 4) Bajar indice unico (EspecieId, Numero), PK vieja y columnas Id/Numero.
            migrationBuilder.Sql("DROP INDEX IX_UnidadesFaenas_EspecieId_Numero ON UnidadesFaenas;");
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas DROP CONSTRAINT PK_UnidadesFaenas;");
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas DROP COLUMN Id;");
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas DROP COLUMN Numero;");

            // 5) Codigo pasa a NOT NULL y PK.
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas ALTER COLUMN Codigo nvarchar(450) NOT NULL;");
            migrationBuilder.Sql("ALTER TABLE UnidadesFaenas ADD CONSTRAINT PK_UnidadesFaenas PRIMARY KEY (Codigo);");

            // 6) FKs: reemplazar columna Guid por la string (rename), NOT NULL, indice y FK a Codigo.
            migrationBuilder.Sql("ALTER TABLE Tipificaciones DROP COLUMN UnidadFaenaId;");
            migrationBuilder.Sql("EXEC sp_rename 'Tipificaciones.UnidadFaenaCodigo', 'UnidadFaenaId', 'COLUMN';");
            migrationBuilder.Sql("ALTER TABLE Tipificaciones ALTER COLUMN UnidadFaenaId nvarchar(450) NOT NULL;");
            migrationBuilder.Sql("CREATE INDEX IX_Tipificaciones_UnidadFaenaId ON Tipificaciones (UnidadFaenaId);");
            migrationBuilder.Sql("ALTER TABLE Tipificaciones ADD CONSTRAINT FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId FOREIGN KEY (UnidadFaenaId) REFERENCES UnidadesFaenas (Codigo);");

            migrationBuilder.Sql("ALTER TABLE Romaneos DROP COLUMN UnidadFaenaId;");
            migrationBuilder.Sql("EXEC sp_rename 'Romaneos.UnidadFaenaCodigo', 'UnidadFaenaId', 'COLUMN';");
            migrationBuilder.Sql("ALTER TABLE Romaneos ALTER COLUMN UnidadFaenaId nvarchar(450) NOT NULL;");
            migrationBuilder.Sql("CREATE INDEX IX_Romaneos_UnidadFaenaId ON Romaneos (UnidadFaenaId);");
            migrationBuilder.Sql("ALTER TABLE Romaneos ADD CONSTRAINT FK_Romaneos_UnidadesFaenas_UnidadFaenaId FOREIGN KEY (UnidadFaenaId) REFERENCES UnidadesFaenas (Codigo);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Romaneos_UnidadesFaenas_UnidadFaenaId",
                table: "Romaneos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId",
                table: "Tipificaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnidadesFaenas",
                table: "UnidadesFaenas");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "UnidadesFaenas");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "UnidadesFaenas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Numero",
                table: "UnidadesFaenas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "UnidadFaenaId",
                table: "Tipificaciones",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UnidadFaenaId",
                table: "Romaneos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnidadesFaenas",
                table: "UnidadesFaenas",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UnidadesFaenas_EspecieId_Numero",
                table: "UnidadesFaenas",
                columns: new[] { "EspecieId", "Numero" },
                unique: true,
                filter: "[FechaBaja] IS NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Romaneos_UnidadesFaenas_UnidadFaenaId",
                table: "Romaneos",
                column: "UnidadFaenaId",
                principalTable: "UnidadesFaenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tipificaciones_UnidadesFaenas_UnidadFaenaId",
                table: "Tipificaciones",
                column: "UnidadFaenaId",
                principalTable: "UnidadesFaenas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
