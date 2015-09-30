namespace RDumont.NugetContentGenerator.Runtime
{
    using System.Text.RegularExpressions;

    public class CsReplacementDefinitionsExtractor : BaseReplacementDefinitionsExtractor
    {
        protected override string GetReplacementLine(string line)
        {
            return line.Trim(' ', '*', '\t');
        }

        protected override bool IsStartOfBlock(string line)
        {
            return Regex.IsMatch(line, @"^\s*/\*\*\s*@pp\s*$");
        }

        protected override bool IsEndOfBlock(string line)
        {
            return line.Trim(' ', '\t').EndsWith("*/");
        }
    }
}