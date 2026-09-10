using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _74_RindeEsperablePorEspecie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Banda de rinde caliente esperable por especie (R-A7). No corrige ni acota el
            // calculo: solo sirve para que el Analisis avise cuando el numero se va de rango,
            // que casi siempre significa un peso de ingreso mal cargado.
            migrationBuilder.AddColumn<double>(
                name: "RindeMaximo",
                table: "Especies",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RindeMinimo",
                table: "Especies",
                type: "float",
                nullable: true);

            // Bandas iniciales, holgadas a proposito: el rinde de referencia ronda 55-58% en
            // bovino y 75-80% en porcino, asi que estas solo se disparan con un numero que ya no
            // puede venir de la hacienda. El SUPERADMIN las ajusta desde la pantalla de Especies,
            // y la especie que las deje vacias no avisa nada.
            migrationBuilder.Sql(@"
                UPDATE dbo.Especies SET RindeMinimo = 45, RindeMaximo = 65 WHERE Codigo = 'V';
                UPDATE dbo.Especies SET RindeMinimo = 65, RindeMaximo = 85 WHERE Codigo = 'P';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RindeMaximo",
                table: "Especies");

            migrationBuilder.DropColumn(
                name: "RindeMinimo",
                table: "Especies");
        }
    }
}
