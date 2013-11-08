namespace RDumont.NugetContentGenerator.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Generator
    {
        public string Generate(string inputPath, string inputContents)
        {
            var extension = GetExtensionFromFilePath(inputPath);
            return Transform(inputContents, extension);
        }

        public Dictionary<string, string> GetReplacements(string definitionsBlock)
        {
            if (string.IsNullOrWhiteSpace(definitionsBlock))
                return new Dictionary<string, string>();

            var lines = definitionsBlock.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^(?<token>\w+):\s+(?<value>.+)$");
                if (!match.Success)
                {
                    throw new InvalidOperationException(
                        string.Format("Invalid replacement definition line: \"{0}\"", line));
                }
                result.Add(match.Groups["token"].Value, match.Groups["value"].Value);
            }

            return result;
        }

        public string Transform(string contents, string extension)
        {
            var extractor = GetExtractorForExtension(extension);
            string definitionsBlock;
            contents = extractor.ExtractReplacementDefinitions(contents, out definitionsBlock);
            var definitions = GetReplacements(definitionsBlock);
            return Replace(contents, definitions);
        }

        private string Replace(string contents, Dictionary<string, string> definitions)
        {
            foreach (var replacement in definitions)
            {
                var token = "$" + replacement.Key + "$";
                var valueToReplace = replacement.Value;
                contents = contents.Replace(valueToReplace, token);
            }
            return contents;
        }

        public string GetExtensionFromFilePath(string filePath)
        {
            return Regex.Match(filePath, "\\w+$").Value;
        }

        private IReplacementDefinitionsExtractor GetExtractorForExtension(string extension)
        {
            switch (extension)
            {
                case "cs":
                    return new CsReplacementDefinitionsExtractor();

                case "cshtml":
                    return new CsHtmlReplacementDefinitionsExtractor();

                case "xml":
                case "config":
                    return new XmlReplacementDefinitionsExtractor();

                default:
                    throw new InvalidOperationException(
                        string.Format("Don't know how to transform file with '.{0}' extension", extension));
            }
        }
    }
}