using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _72_MotivoDecomisoExigeContusion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Marca del motivo que describe un golpe (R-E26). Si esta prendida, la media res que
            // se decomisa por ese motivo no puede quedar registrada como "sin contusion".
            //
            // Es una marca del catalogo y no una lista de codigos escrita en el handler, por la
            // misma razon que los datos del palco: el nomenclador lo administra el SUPERADMIN y
            // cada especie puede nombrar sus motivos como quiera.
            migrationBuilder.AddColumn<bool>(
                name: "ExigeContusion",
                table: "MotivosDecomisos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // El seed va con la columna y no en una migracion aparte: sin esta fila marcada, la
            // regla no corre para ningun motivo y la columna no significa nada. Hoy el unico
            // motivo de golpe cargado es 'CONT' (Contusiones) de vacuno, de la migracion 71.
            migrationBuilder.Sql(@"
                UPDATE dbo.MotivosDecomisos
                SET ExigeContusion = 1
                WHERE Codigo = 'CONT' AND EspecieId = 'V';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExigeContusion",
                table: "MotivosDecomisos");
        }
    }
}
