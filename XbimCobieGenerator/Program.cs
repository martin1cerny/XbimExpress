using System;
using System.Diagnostics;
using System.IO;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace XbimCobieGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();

            //set working directory
            Environment.CurrentDirectory = @"c:\Users\Martin\Source\Repos\XbimCobieExpress";


            var cobie = SchemaModel.Load(File.ReadAllText("COBieExpress.exp"), "COBIE");

            //Change names to prevent name clashes
            foreach (var entity in cobie.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CobieExpress",
            };

            Generator.GenerateSchema(settings, cobie);
            Console.WriteLine(@"COBieExpress generated");


            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds / 1000);
            Console.Beep(440, 500);
            Console.ReadKey();
        }
    }
}
