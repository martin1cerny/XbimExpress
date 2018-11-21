using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;
using Xbim.ExpressParser;

namespace Xbim.CodeGeneration.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void GenerateIfc2X3()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim2.Ifc2x3"
            };
            var schema = SchemaModel.LoadIfc2x3();

            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateIfc2X3WithCommons()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3"
            };
            var schema = SchemaModel.LoadIfc2x3();

            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateIfc4WithCommons()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc4(),
                OutputPath = "Xbim.Ifc4"
            };
            var schema = SchemaModel.LoadIfc4();

            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateCobieExpress()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CobieExpress"
            };
            var schema = SchemaModel.Load(Schemas.COBieExpress, SchemaSources.COBIE);
            foreach (var entity in schema.Get<EntityDefinition>())
            {
                entity.Name = "Cobie" + entity.Name;
            }

            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateCisAsInterfaces()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CIS2"
            };
            var schema = SchemaModel.LoadCis2();

            //change names to be more like C#
            ProcessNames(schema, "Cis");
            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateStepAsInterfaces()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.Step42",
            };
            var schema = SchemaModel.LoadStepGeometry();

            //change names to be more like C#
            ProcessNames(schema, "Stp");
            Generator.GenerateSchema(settings, schema);
        }

        private static void ProcessNames(SchemaModel model, string prefix)
        {
            //change names to be more like C#
            foreach (var type in model.Get<NamedType>())
            {
                type.Name = prefix + MakeCamelCaseFromUnderscore(type.Name);

                var entity = type as EntityDefinition;
                if(entity == null) continue;

                foreach (var attribute in entity.Attributes)
                {
                    attribute.Name = MakeCamelCaseFromUnderscore(attribute.Name);
                }
            }
        }

        private static string MakeCamelCaseFromUnderscore(string value)
        {
            var parts = value.Split('_');
            var upper = parts.Select(p => p.First().ToString().ToUpper() + p.Substring(1).ToLower());
            return String.Join("", upper);
        }
    }
}
