using RDumont.NugetContentGenerator.Runtime.Extractors;

namespace RDumont.NugetContentGenerator.Runtime.Tests
{
    public class TestableCsExtractor : CsReplacementDefinitionsExtractor
    {
        public new string GetReplacementLine(string line)
        {
            return base.GetReplacementLine(line);
        }

        public new bool IsStartOfBlock(string line)
        {
            return base.IsStartOfBlock(line);
        }

        public new bool IsEndOfBlock(string line)
        {
            return base.IsEndOfBlock(line);
        }
    }
}
