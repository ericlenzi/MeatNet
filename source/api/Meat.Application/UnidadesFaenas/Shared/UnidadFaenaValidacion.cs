using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.Shared
{
    /// <summary>
    /// Validaciones de la unidad de faena, compartidas por el alta y la edicion.
    ///
    /// El punto de fondo: CantidadCuartos y PiezasPorAnimal no son preferencias de la empresa,
    /// son la aritmetica de como se presenta la res. Una media res son dos piezas por animal
    /// siempre, en cualquier planta. Antes lo unico que se pedia era PiezasPorAnimal >= 1, y con
    /// eso una unidad mal cargada llegaba hasta el Tipificador: CrearRomaneo exige exactamente
    /// PiezasPorAnimal piezas por animal, asi que "1/2 RES" con 3 piezas hacia que el operador
    /// tuviera que colgar y pesar una tercera media res que no existe.
    /// </summary>
    public static class UnidadFaenaValidacion
    {
        /// <summary>Un animal tiene cuatro cuartos. Es el tope de todo lo que sigue.</summary>
        private const int CuartosPorAnimal = 4;

        public static async Task ValidarAsync(
            MeatContext context,
            string especieId,
            string nombre,
            int cantidadCuartos,
            int piezasPorAnimal,
            string tipoMaterialId,
            CancellationToken cancellationToken)
        {
            if (!await context.Especies.AnyAsync(e => e.Codigo == especieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidationException("El nombre es requerido.");

            if (piezasPorAnimal < 1 || piezasPorAnimal > CuartosPorAnimal)
                throw new ValidationException(
                    $"Las piezas por animal deben estar entre 1 y {CuartosPorAnimal}: son las piezas de esta "
                    + "unidad que salen de un mismo animal (1 RES = 1, 1/2 RES = 2, 1/4 = 4).");

            if (cantidadCuartos < 0 || cantidadCuartos > CuartosPorAnimal)
                throw new ValidationException(
                    $"Los cuartos deben estar entre 0 y {CuartosPorAnimal}. Se usa 0 para las unidades de decomiso, "
                    + "que no se despiezan.");

            // La cuenta que ata las dos columnas: si cada pieza representa N cuartos y de un animal
            // salen M piezas, no pueden sumar mas de los cuatro cuartos que tiene el animal.
            if (cantidadCuartos * piezasPorAnimal > CuartosPorAnimal)
                throw new ValidationException(
                    $"La combinacion no cierra: {piezasPorAnimal} pieza(s) de {cantidadCuartos} cuarto(s) suman "
                    + $"{cantidadCuartos * piezasPorAnimal} cuartos y un animal tiene {CuartosPorAnimal}. "
                    + "Revise cuantos cuartos representa la unidad y cuantas piezas de esa unidad salen de un animal.");

            // Requerido: es el eje de forma que alinea la unidad con el catalogo de Materiales.
            // Si viene vacio, la validacion de consistencia de la Tipificacion se saltea en
            // silencio y una media res puede terminar tipificada contra un material de cuarto.
            if (string.IsNullOrWhiteSpace(tipoMaterialId))
                throw new ValidationException(
                    "El tipo de material es requerido: es lo que alinea la unidad de faena con el catalogo de "
                    + "materiales y permite validar que la tipificacion apunte a un material de la misma forma.");

            if (!await context.TiposMateriales.AnyAsync(t => t.Codigo == tipoMaterialId, cancellationToken))
                throw new ValidationException("El tipo de material indicado no existe.");
        }
    }
}
