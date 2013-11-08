using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDumont.NugetContentGenerator.Runtime.Tests
{
    public class TestableXmlExtractor : XmlReplacementDefinitionsExtractor
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
