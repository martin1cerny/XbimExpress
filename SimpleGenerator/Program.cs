using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace SimpleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {

                var wd = args[0];
                if (!Directory.Exists(wd))
                    Directory.CreateDirectory(wd);
                Environment.CurrentDirectory = wd;
            }

            var schemas = Directory
                .EnumerateFiles(Environment.CurrentDirectory, "*.exp", SearchOption.TopDirectoryOnly)
                .ToList();
            if (schemas.Count == 0)
            {
                Console.WriteLine($"There are no EXPRESS files in '{Environment.CurrentDirectory}'");
                return;
            }

            var watch = Stopwatch.StartNew();
            foreach (var schemaFile in schemas)
            {
                var ns = Path.GetFileNameWithoutExtension(schemaFile);
                if (!ns.StartsWith("Xbim."))
                    ns = "Xbim." + ns;

                if (!File.Exists(schemaFile))
                {
                    Console.WriteLine($"Schema file '{schemaFile}' doesn't exist.");
                    return;
                }

                var settings = new GeneratorSettings
                {
                    OutputPath = ns
                };



                var schemaData = File.ReadAllText(schemaFile);
                var schema = SchemaModel.Load(schemaData, ns);

                Generator.GenerateSchema(settings, schema);
                Console.WriteLine($"Schema {schema.FirstSchema.Name} generated.");

            }
            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds / 1000);
            Console.Beep(440, 500);
        }
    }
}
