namespace RDumont.NugetContentGenerator.Runtime
{
    public interface IReplacementDefinitionsExtractor
    {
        string ExtractReplacementDefinitions(string originalText, out string replacementDefinitions);
    }
}
