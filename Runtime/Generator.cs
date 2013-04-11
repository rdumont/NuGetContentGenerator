using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace RDumont.NugetContentGenerator.Runtime
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class Generator : IVsSingleFileGenerator
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".pp";
            return pbstrDefaultExtension.Length;
        }

        public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
        {
            try
            {
                var extension = GetExtensionFromFilePath(wszInputFilePath);
                var contents = Transform(bstrInputFileContents, extension);

                var bytes = Encoding.UTF8.GetBytes(contents);
                var length = bytes.Length;
                rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
                Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);
                pcbOutput = (uint) length;

                return VSConstants.S_OK;
            }
            catch (Exception ex)
            {
                pGenerateProgress.GeneratorError(0, 1, ex.ToString(), 0, 0);
                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }
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

        public Dictionary<string, string> GetReplacements(string definitionsBlock)
        {
            var lines = definitionsBlock.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
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

        private IReplacementDefinitionsExtractor GetExtractorForExtension(string extension)
        {
            switch (extension)
            {
                case "cs":
                    return new CsReplacementDefinitionsExtractor();

                default:
                    throw new InvalidOperationException(
                        string.Format("Don't know how to transform file with '.{0}' extension", extension));
            }
        }

        public string GetExtensionFromFilePath(string filePath)
        {
            return Regex.Match(filePath, "\\w+$").Value;
        }
    }
}
