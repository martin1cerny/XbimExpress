using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace SimpleGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Not enough arguments.");
                return;
            }

            var wd = args[0];
            if (!Directory.Exists(wd))
                Directory.CreateDirectory(wd);
            Environment.CurrentDirectory = wd;

            var schemas = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.exp", SearchOption.TopDirectoryOnly);

            foreach (var schemaFile in schemas)
            {
                var ns = Path.GetFileNameWithoutExtension(schemaFile);

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
            }
        }
    }
}
