using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Meat.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class _73_MediaResCondenada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Tercer nivel de decomiso (R-E27): la media res condenada entera. Entre la res
            // condenada (Romaneo.DecomisoTotal) y el recorte de kilos (PesoDecomisado), que no
            // cubrian el caso de condenar una media res dejando la otra en pie.
            //
            // No lleva columna de kilos propia: sus kilos condenados son el Peso de la pieza, y
            // guardar el mismo numero dos veces solo abre la puerta a que difieran. El motivo
            // reusa MotivoDecomisoId, que ya estaba en la pieza para el recorte.
            migrationBuilder.AddColumn<bool>(
                name: "Decomisada",
                table: "RomaneosPiezas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decomisada",
                table: "RomaneosPiezas");
        }
    }
}
