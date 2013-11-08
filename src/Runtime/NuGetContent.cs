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

        public string[] Extensions { get; set; }

        public override bool Execute()
        {
            var generator = new Generator();
            var root = Environment.CurrentDirectory;

            if (Extensions == null || Extensions.Length == 0)
                throw new ArgumentNullException("Extensions", "Please include at least one extension eg. <Extension Include='.cs'>");
            
            foreach (var file in Files.Where(f => Extensions.Any(ex => f.EndsWith(ex))))
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
