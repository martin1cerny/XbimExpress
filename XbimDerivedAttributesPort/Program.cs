using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XbimDerivedAttributesPort
{
    class Program
    {
        static void Main(string[] args)
        {
            const string inputDir = @"c:\CODE\XbimGit\XbimEssentials\Xbim.Ifc2x3";
            const string outputDir = @"c:\CODE\XbimGit\XbimEssentials\Xbim.Ifc4";

            var inFiles = Directory.GetFiles(inputDir, "Ifc*.cs", SearchOption.AllDirectories).Where(f => !f.Contains("Interfaces")).ToList();
            var inNames = inFiles.ToDictionary(f => Path.GetFileNameWithoutExtension(f)??"");
            var outFiles = Directory.GetFiles(outputDir, "*.cs", SearchOption.AllDirectories);

            //find matching files
            foreach (var outFile in outFiles)
            {
                var outName = Path.GetFileNameWithoutExtension(outFile);
                if(outName == null) continue;
                string inFile;
                if (!inNames.TryGetValue(outName, out inFile))
                    continue;

                var inData = File.ReadAllText(inFile);
                var inSections = GetSections(inData).Where(s => s.Key.StartsWith("Getter for")).ToList();
                if (!inSections.Any()) continue;

                var outData = File.ReadAllText(outFile);
                var outSections = GetSections(outData).Where(s => s.Key.StartsWith("Getter for")).ToList();
                if (!outSections.Any()) continue;

                var changed = false;
                foreach (var outSection in outSections)
                {
                    var inSectionData = inSections.FirstOrDefault(kvp => kvp.Key == outSection.Key).Value;
                    if (inSectionData == null) continue;

                    //put in IFC2x3 implementation
                    outData = outData.Replace(outSection.Value, inSectionData);
                    changed = true;
                }

                if (changed)
                    File.WriteAllText(outFile, outData);
            }

        }

        private static readonly Regex CustomCodeRegex = new Regex("(//##).*?(//##)", RegexOptions.Singleline);

        private static Dictionary<string, string> GetSections(string content)
        {
            var result = new Dictionary<string, string>();
            var sections = CustomCodeRegex.Matches(content);
            foreach (Match section in sections)
            {
                var fli = section.Value.IndexOf('\n');
                var name = section.Value.Substring(0, fli).TrimStart('/', '#').Trim();
                result.Add(name, section.Value);
            }
            return result;
        }
    }
}
