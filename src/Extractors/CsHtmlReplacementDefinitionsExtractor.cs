using System.Text.RegularExpressions;

namespace RDumont.NugetContentGenerator.Runtime.Extractors
{
    public class CsHtmlReplacementDefinitionsExtractor : BaseReplacementDefinitionsExtractor
    {
        protected override string GetReplacementLine(string line)
        {
            return line.Trim(' ', '*', '\t');
        }

        protected override bool IsStartOfBlock(string line)
        {
            return Regex.IsMatch(line, @"^\s*@\*\s*@pp\s*$");
        }

        protected override bool IsEndOfBlock(string line)
        {
            return line.Trim(' ', '\t').EndsWith("*@");
        }
    }
}
