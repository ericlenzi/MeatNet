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

        // Letras de las piezas (medias reses) para vacunos.
        public static readonly string[] Letras = { "A", "B", "C", "D" };
    }
}
