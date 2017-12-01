using System.IO;
using System.Text;

namespace RDumont.NugetContentGenerator.Runtime.Extractors
{
    public abstract class BaseReplacementDefinitionsExtractor : IReplacementDefinitionsExtractor
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

        protected abstract string GetReplacementLine(string line);

        protected abstract bool IsStartOfBlock(string line);

        protected abstract bool IsEndOfBlock(string line);
    }
}
