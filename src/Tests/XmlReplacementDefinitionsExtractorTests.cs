using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDumont.NugetContentGenerator.Runtime.Tests
{
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    [TestFixture]
    public class XmlReplacementDefinitionsExtractorTests
    {
        [Test]
        public void Extract_definitions_block()
        {
            // Arrange
            var text = @"<!-- @pp
  replacement 1
  replacement 2
 -->
contents";
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            string definitions;
            var result = extractor.ExtractReplacementDefinitions(text, out definitions);

            // Assert
            Assert.That(result, Is.EqualTo("contents"));
            Assert.That(definitions, Is.EqualTo("replacement 1\r\nreplacement 2\r\n"));
        }

        [Test]
        public void Extract_definitions_block_after_white_lines()
        {
            // Arrange
            var text = @"

<!-- @pp
 replacement 1
 replacement 2
-->
contents";
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            string definitions;
            var result = extractor.ExtractReplacementDefinitions(text, out definitions);

            // Assert
            Assert.That(result, Is.EqualTo("contents"));
            Assert.That(definitions, Is.EqualTo("replacement 1\r\nreplacement 2\r\n"));
        }

        [Test]
        public void Should_find_definitions_after_content()
        {
            // Arrange
            var text = @"
<xml>
<!-- @pp
  replacement 1
  replacement 2
 -->
</xml>";
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            string definitions;
            var result = extractor.ExtractReplacementDefinitions(text, out definitions);

            // Assert
            Assert.That(result, Is.EqualTo("<xml>\r\n</xml>"));
            Assert.That(definitions, Is.EqualTo("replacement 1\r\nreplacement 2\r\n"));
        }

        [TestCase("replacement line")]
        [TestCase("  replacement line")]
        [TestCase(" \t replacement line")]
        [TestCase("\t  replacement line")]
        public void Get_replacement_line(string originalLine)
        {
            // Arrange
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            var result = extractor.GetReplacementLine(originalLine);

            // Assert
            Assert.That(result, Is.EqualTo("replacement line"));
        }

        [TestCase("<!-- @pp")]
        [TestCase("<!-- @pp  ")]
        [TestCase("<!--   @pp  ")]
        [TestCase("  <!--   @pp")]
        [TestCase(" \t <!-- @pp")]
        public void Is_start_of_block(string originalLine)
        {
            // Arrange
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            var isStartOfBlock = extractor.IsStartOfBlock(originalLine);

            Console.WriteLine(originalLine.Trim(' ', '\t'));

            // Assert
            Assert.That(isStartOfBlock, Is.True);
        }

        [TestCase("-->")]
        [TestCase("--->")]
        [TestCase(" -->  ")]
        [TestCase("\t-->")]
        public void Is_end_of_block(string originalLine)
        {
            // Arrange
            var extractor = new XmlReplacementDefinitionsExtractor();

            // Act
            var isEndOfBlock = extractor.IsEndOfBlock(originalLine);

            Console.WriteLine(originalLine.Trim(' ', '\t'));

            // Assert
            Assert.That(isEndOfBlock, Is.True);
        }
    }
}
