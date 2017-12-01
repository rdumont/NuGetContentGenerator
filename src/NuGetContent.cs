namespace RDumont.NugetContentGenerator.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class NuGetContent : Task
    {
        [Required]
        public string[] Files { get; set; }

        public override bool Execute()
        {
            var generator = new Generator();
            var root = Environment.CurrentDirectory;
            
            foreach (var file in Files.Where(f => f.EndsWith(".cs")))
            {
                var fullPath = Path.Combine(root, file);
                var contents = File.ReadAllText(fullPath);
                
                var output = generator.Generate(fullPath, contents);
                var targetPath = file + ".pp";

                File.WriteAllText(Path.Combine(root, targetPath), output);
            }

            return true;
        }
    }
}
