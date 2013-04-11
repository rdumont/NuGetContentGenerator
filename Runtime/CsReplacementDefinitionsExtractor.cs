namespace RDumont.NugetContentGenerator.Runtime
{
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class CsReplacementDefinitionsExtractor : IReplacementDefinitionsExtractor
    {
        public string ExtractReplacementDefinitions(string originalText, out string replacementDefinitions)
        {
            var reader = new StringReader(originalText);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (!IsStartOfBlock(line)) break;

                var definitionsBuilder = new StringBuilder();
                ReadReplacements(reader, definitionsBuilder);
                replacementDefinitions = definitionsBuilder.ToString();
                return reader.ReadToEnd();
            }
            replacementDefinitions = null;
            return originalText;
        }

        private void ReadReplacements(StringReader reader, StringBuilder definitions)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (IsEndOfBlock(line)) return;
                definitions.AppendLine(GetReplacementLine(line));
            }
        }

        protected static string GetReplacementLine(string line)
        {
            return line.Trim(' ', '*', '\t');
        }

        protected static bool IsStartOfBlock(string line)
        {
            return Regex.IsMatch(line, @"^\s*/\*\*\s*@pp\s*$");
        }

        protected static bool IsEndOfBlock(string line)
        {
            return line.Trim(' ', '\t').EndsWith("*/");
        }
    }
}