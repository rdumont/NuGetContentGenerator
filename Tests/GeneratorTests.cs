using System.Collections.Generic;
using NUnit.Framework;

namespace RDumont.NugetContentGenerator.Runtime.Tests
{
    [TestFixture]
    public class GeneratorTests
    {
        [Test]
        public void Get_extension_from_file_path()
        {
            // Arrange
            var path = "/path/to/some/file.abc.cs";
            var generator = new Generator();

            // Act
            var extension = generator.GetExtensionFromFilePath(path);

            // Assert
            Assert.That(extension, Is.EqualTo("cs"));
        }

        [Test]
        public void Get_replacements()
        {
            // Arrange
            var text = @"rootnamespace: bla bla bla
otherToken: some value";
            var generator = new Generator();

            // Act
            var replacements = generator.GetReplacements(text);

            // Assert
            Assert.That(replacements, Is.EquivalentTo(new Dictionary<string, string>
            {
                {"rootnamespace", "bla bla bla"},
                {"otherToken", "some value"}
            }));
        }

        [Test]
        public void Wrong_replacement_syntax_should_throw_error()
        {
            // Arrange
            var text = @"rootnamespace: bla bla bla
not a good line";
            var generator = new Generator();

            // Act & Assert
            Assert.That(() => generator.GetReplacements(text),
                Throws.InvalidOperationException.With.Message
                    .EqualTo("Invalid replacement definition line: \"not a good line\""));
        }
    }
}
