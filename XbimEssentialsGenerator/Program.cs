using System;
using System.Collections.Generic;
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
            //set working directory if specified.
            if (args.Length > 0) Environment.CurrentDirectory = args[0];

            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Common"
            };
            var schema = SchemaModel.LoadIfc2x3();
            Generator.Generate(settings, schema);

            settings.Structure = DomainStructure.LoadIfc4();
            settings.OutputPath = "Xbim.Ifc4";
            schema = SchemaModel.LoadIfc4();
            Generator.Generate(settings, schema);

            settings.Structure = null;
            settings.OutputPath = "Xbim.CobieExpress";
            schema = SchemaModel.Load(Schemas.COBieExpress);
            //Change names to prevent name clashes
            foreach (var entity in schema.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;
            Generator.Generate(settings, schema);
       
        }
    }
}
