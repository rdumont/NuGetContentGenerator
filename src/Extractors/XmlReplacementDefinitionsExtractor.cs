using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RDumont.NugetContentGenerator.Runtime.Extractors
{
    public class XmlReplacementDefinitionsExtractor : IReplacementDefinitionsExtractor
    {
        public string ExtractReplacementDefinitions(string originalText, out string replacementDefinitions)
        {
            var reader = new StringReader(originalText);
            var contentBuilder = new StringBuilder();
            var definitionsFound = false;
            string foundDefinitions = null;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (IsStartOfBlock(line))
                {
                    if (definitionsFound)
                        throw new ArgumentException("Multiple definitions found");

                    var definitionsBuilder = new StringBuilder();
                    ReadReplacements(reader, definitionsBuilder);
                    foundDefinitions = definitionsBuilder.ToString();

                    definitionsFound = true;
                }
                else
                {
                    contentBuilder.AppendLine(line);
                }
            }

            replacementDefinitions = foundDefinitions;

            if (definitionsFound)
            {
                return contentBuilder.ToString().Trim();
            }
            else
            {
                return originalText;
            }
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

        public string GetReplacementLine(string line)
        {
            return line.Trim(' ', '\t');
        }

        public bool IsStartOfBlock(string line)
        {
            return Regex.IsMatch(line, @"^\s*<!--\s*@pp\s*$");
        }

        public bool IsEndOfBlock(string line)
        {
            return line.Trim(' ', '\t').EndsWith("-->");
        }
    }
}
