using System;
using NUnit.Framework;
using RDumont.NugetContentGenerator.Runtime.Extractors;

namespace RDumont.NugetContentGenerator.Runtime.Tests
{
    [TestFixture]
    public class CsReplacementDefinitionsExtractorTests
    {
        [Test]
        public void Extract_definitions_block()
        {
            // Arrange
            var text = @"/** @pp
 * replacement 1
 * replacement 2
 */
contents";
            var extractor = new CsReplacementDefinitionsExtractor();

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

/** @pp
 * replacement 1
 * replacement 2
 */
contents";
            var extractor = new CsReplacementDefinitionsExtractor();

            // Act
            string definitions;
            var result = extractor.ExtractReplacementDefinitions(text, out definitions);

            // Assert
            Assert.That(result, Is.EqualTo("contents"));
            Assert.That(definitions, Is.EqualTo("replacement 1\r\nreplacement 2\r\n"));
        }

        [Test]
        public void Should_not_find_definitions_after_content()
        {
            // Arrange
            var text = @"
this breaks the definitions
/** @pp
 * replacement 1
 * replacement 2
 */
contents";
            var extractor = new CsReplacementDefinitionsExtractor();

            // Act
            string definitions;
            var result = extractor.ExtractReplacementDefinitions(text, out definitions);

            // Assert
            Assert.That(result, Is.EqualTo(text));
            Assert.That(definitions, Is.EqualTo(null));
        }

        [TestCase("replacement line")]
        [TestCase("  replacement line")]
        [TestCase(" \t replacement line")]
        [TestCase("*replacement line")]
        [TestCase("  *replacement line")]
        [TestCase("  **replacement line")]
        [TestCase("*   replacement line")]
        [TestCase("\t*  replacement line")]
        public void Get_replacement_line(string originalLine)
        {
            // Arrange
            var extractor = new TestableCsExtractor();

            // Act
            var result = extractor.GetReplacementLine(originalLine);

            // Assert
            Assert.That(result, Is.EqualTo("replacement line"));
        }

        [TestCase("/** @pp")]
        [TestCase("/** @pp  ")]
        [TestCase("/**   @pp  ")]
        [TestCase("  /**   @pp")]
        [TestCase(" \t /** @pp")]
        public void Is_start_of_block(string originalLine)
        {
            // Arrange
            var extractor = new TestableCsExtractor();

            // Act
            var isStartOfBlock = extractor.IsStartOfBlock(originalLine);

            Console.WriteLine(originalLine.Trim(' ', '\t'));

            // Assert
            Assert.That(isStartOfBlock, Is.True);
        }

        [TestCase("*/")]
        [TestCase("**/")]
        [TestCase(" */  ")]
        [TestCase("\t*/")]
        public void Is_end_of_block(string originalLine)
        {
            // Arrange
            var extractor = new TestableCsExtractor();

            // Act
            var isEndOfBlock = extractor.IsEndOfBlock(originalLine);

            Console.WriteLine(originalLine.Trim(' ', '\t'));

            // Assert
            Assert.That(isEndOfBlock, Is.True);
        }
    }
}
