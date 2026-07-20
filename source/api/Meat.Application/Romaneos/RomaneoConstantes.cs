namespace Meat.Application.Romaneos
{
    /// <summary>Especies soportadas por la Ejecucion de Faena (Fase 2 - MVP).</summary>
    public static class EspeciesFaena
    {
        public const string Vacuno = "V";
        public const string Porcino = "P";

        public static bool EsSoportada(string especieId) =>
            especieId == Vacuno || especieId == Porcino;
    }

    /// <summary>Codigos usados por el romaneo.</summary>
    public static class RomaneoConstantes
    {
        // Codigo del TipoMedicion que representa el peso (catalogo TiposMediciones).
        public const string MedicionPeso = "PESO";

        // TipoNumerador (y Codigo) del Numerador que lleva la secuencia de romaneos.
        public const string TipoNumeradorRomaneo = "ROMANEO";

        // Letras de las piezas (medias reses) para vacunos.
        public static readonly string[] Letras = { "A", "B", "C", "D" };

        // Cuartos de una res entera. Nº de piezas por animal = CuartosPorAnimal / UnidadFaena.CantidadCuartos
        // (RES=4 -> 1 pieza; MEDIA RES=2 -> 2 piezas A/B; CUARTO=1 -> 4 piezas).
        public const int CuartosPorAnimal = 4;

        public static int PiezasPorAnimal(int cantidadCuartos) =>
            cantidadCuartos > 0 ? System.Math.Max(1, (int)System.Math.Round((double)CuartosPorAnimal / cantidadCuartos)) : 1;
    }
}
