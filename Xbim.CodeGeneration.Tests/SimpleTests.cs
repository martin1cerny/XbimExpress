using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.Schemas;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

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
                OutputPath = "Xbim.Ifc2x3"
            };
            var schema = SchemaModel.LoadIfc2x3();

            Generator.Generate(settings, schema);
        }

        [TestMethod]
        public void GenerateIfc2X3WithCommons()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Commons"
            };
            var schema = SchemaModel.LoadIfc2x3();

            Generator.Generate(settings, schema);
        }

        [TestMethod]
        public void GenerateIfc4WithCommons()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc4(),
                OutputPath = "Xbim.Ifc4",
                InfrastructureOutputPath = "Xbim.Commons"
            };
            var schema = SchemaModel.LoadIfc4();

            Generator.Generate(settings, schema);
        }

        [TestMethod]
        public void GenerateCobieExpress()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CobieExpress",
                InfrastructureOutputPath = "Xbim.Commons"
            };
            var schema = SchemaModel.Load(Schemas.COBieExpress);
            foreach (var entity in schema.Schema.Entities)
            {
                entity.Name = "Cobie" + entity.Name;
            }

            Generator.Generate(settings, schema);
        }

        [TestMethod]
        public void GenerateCis()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CIS2"
            };
            var schema = SchemaModel.LoadCis2();

            //change names to be more like C#
            foreach (var type in schema.Get<NamedType>())
            {
                var name = type.Name.ToString();
                var parts = name.Split('_');
                var upper = parts.Select(p => p.First().ToString().ToUpper() + p.Substring(1).ToLower());
                type.Name = "Cis" + String.Join("", upper);
            }

            Generator.Generate(settings, schema);
        }
    }
}
