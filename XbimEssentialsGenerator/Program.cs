using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace XbimEssentialsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            //set working directory if specified.
            if (args.Length > 0) Environment.CurrentDirectory = args[0];

            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Common"
            };
            var schema = SchemaModel.LoadIfc2x3();
            SetTypeNumbersForIfc2X3(schema);
            Generator.Generate(settings, schema);
            Console.WriteLine(@"IFC2x3 generated");

            settings.Structure = DomainStructure.LoadIfc4();
            settings.OutputPath = "Xbim.Ifc4";
            schema = SchemaModel.LoadIfc4();
            Generator.Generate(settings, schema);
            Console.WriteLine(@"IFC4 generated");

            settings.Structure = null;
            settings.OutputPath = "Xbim.CobieExpress";
            schema = SchemaModel.Load(Schemas.COBieExpress);
            //Change names to prevent name clashes
            foreach (var entity in schema.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;
            Generator.Generate(settings, schema);
            Console.WriteLine(@"COBieExpress generated");

            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds/1000);
            Console.Beep(440, 1000);
            Console.ReadKey();

        }

        private static void SetTypeNumbersForIfc2X3(SchemaModel model)
        {
            var entities = model.Get<EntityDefinition>().ToList();
            foreach (var definition in entities)
            {
                IfcEntityNameEnum entityEnum;
                if (Enum.TryParse(definition.PersistanceName, true, out entityEnum))
                {
                    definition.TypeId = (short)entityEnum;
                }
                else
                {
                    Console.WriteLine(@"Type not found: " + definition.PersistanceName); 
                }
            }
        }
    }
}
