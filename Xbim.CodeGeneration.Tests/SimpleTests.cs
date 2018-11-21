using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;
using Xbim.ExpressParser;
using System.IO;
using System.Diagnostics;

namespace Xbim.CodeGeneration.Tests
{
    [TestClass]
    [DeploymentItem("Schemas")]
    public class SimpleTests
    {
        [TestMethod]
        public void ParseIfc2x3()
        {
            var parser = new ExpressParser.ExpressParser();
            var result = parser.Parse(File.ReadAllText("IFC2X3_TC1.exp"), SchemaSources.IFC2x3_TC1);
            Assert.IsTrue(result);


            var wall = parser.SchemaInstance.Get<EntityDefinition>(d => d.Name == "IfcWall").FirstOrDefault();
            Assert.IsNotNull(wall, "There should be a definition of IfcWall entity.");
            var wallExplicitAttrs = wall.AllExplicitAttributes.ToList();
            Assert.IsTrue(wallExplicitAttrs.Any(), "There should be some explicit attributes for a wall.");
            Assert.AreEqual(8, wallExplicitAttrs.Count, "There should be 8 explicit attributes for a wall.");

            var bTypes = parser.SchemaInstance.Get<DefinedType>(t => t.Domain is BooleanType).ToList();
            var bAttributes = parser.SchemaInstance.Get<ExplicitAttribute>(t => t.Domain is BooleanType || bTypes.Contains(t.Domain));
            foreach (var attribute in bAttributes)
            {
                var typeName = attribute.Domain is SimpleType
                    ? attribute.Domain.GetType().Name
                    : ((NamedType)attribute.Domain).Name.ToString();
                Debug.WriteLine("{0} -> {1} ({2})", attribute.ParentEntity.Name, attribute.Name, typeName);
            }
        }

        [TestMethod]
        public void GenerateIfc2X3()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3"
            };
            var schema = SchemaModel.Load(File.ReadAllText("IFC2X3_TC1.exp"), "IFC2X3_TC1");


            Generator.GenerateSchema(settings, schema);
        }

        [TestMethod]
        public void GenerateIfc4()
        {
            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc4"
            };
            var schema = SchemaModel.Load(File.ReadAllText("IFC4_ADD2.exp"), "IFC4_ADD2");

            Generator.GenerateSchema(settings, schema);
        }


        [TestMethod]
        public void GenerateCobieExpress()
        {
            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.CobieExpress"
            };
            var schema = SchemaModel.Load(File.ReadAllText("COBieExpress.exp"), "COBieExpress");
            foreach (var entity in schema.Get<EntityDefinition>())
            {
                entity.Name = "Cobie" + entity.Name;
            }

            Generator.GenerateSchema(settings, schema);
        }
    }
}
