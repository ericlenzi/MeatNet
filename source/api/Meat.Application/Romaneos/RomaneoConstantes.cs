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
        // Codigo de la magnitud que representa el peso (catalogo TiposMagnitudes): que se mide.
        // Con que se mide (manual, balanza, automatico) es el TipoMedicion de la cabecera.
        public const string MagnitudPeso = "PESO";

        // Letras de las piezas (medias reses) para vacunos.
        public static readonly string[] Letras = { "A", "B", "C", "D" };
    }
}
