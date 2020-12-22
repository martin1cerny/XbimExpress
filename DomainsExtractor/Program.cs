using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Xbim.IfcDomains;

namespace DomainsExtractor
{
    /// <summary>
    /// This utility extracts domains structure from HTML documentation. EXPRESS schema file 
    /// doesn't contain any namespaces. But IFC documentation is divided into sections by topic
    /// which makes it easier for orientation. These domains are used to set up namespaces
    /// in code generator. It would also be possible to create this domains structure from IFCDOC
    /// definition file as an alternative.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var inputs = new[] {
                // @"c:\IFC_2x3_TC1",
                // @"c:\Users\Martin\Source\IFC4_ADD2\schema"
                // @"c:\Users\Martin\Source\IFC4x3_RC1\RC1\HTML\schema"
                @"c:\Users\Martin\Source\IFC4x3_RC2\html\schema"
            };

            foreach (var input in inputs)
            {
                var schemaName = input.Split('\\').First(s => s.Contains("IFC"));
                var schema = new DomainStructure() { Name = schemaName, Domains = new List<Domain>() };

                var dirs = Directory.EnumerateDirectories(input).Where(d => NameContainsIfc(d));
                foreach (var dir in dirs)
                {
                    var contentFile = Path.Combine(dir, "content.htm");
                    var domainName = GetTitle(contentFile).Substring(3);

                    var domain = new Domain() { Name = domainName, Types = new List<string>() };
                    schema.Domains.Add(domain);
                    var domainTypesDir = Path.Combine(dir, "lexical");

                    foreach (var file in Directory.EnumerateFiles(domainTypesDir))
                    {
                        var typeName = GetTitle(file);
                        domain.Types.Add(typeName);
                    }
                }

                using (var writer = new XmlTextWriter(schemaName + ".xml", Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    var serializer = new XmlSerializer(typeof(DomainStructure));
                    serializer.Serialize(writer, schema);
                }
            }

        }

        public static string GetTitle(string htmlFile)
        {
            if (!File.Exists(htmlFile))
                throw new ArgumentException("htmlFile");

            var text = File.ReadAllText(htmlFile);
            var rgx = new Regex("(?<=<title>)[a-zA-Z0-9]*(?=</title>)", RegexOptions.IgnoreCase);
            return rgx.Match(text).Value;
        }

        public static bool NameContainsIfc(string dirPath)
        {
            var name = dirPath.Split(Path.DirectorySeparatorChar).Last();
            return name.StartsWith("ifc", true, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}