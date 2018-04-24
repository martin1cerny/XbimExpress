using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;

namespace Xbim.ExpressParser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseIfc2x3()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.IFC2X3_TC1, SchemaSources.IFC2x3_TC1);
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
                    : ((NamedType) attribute.Domain).Name.ToString();
                Debug.WriteLine("{0} -> {1} ({2})", attribute.ParentEntity.Name, attribute.Name, typeName);
            }
        }

        [TestMethod]
        public void ParseIfc4()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.IFC4, SchemaSources.IFC4);
            Assert.IsTrue(result);

            var type =
                parser.SchemaInstance.Get<DefinedType>(t => t.Name == "IfcPropertySetDefinitionSet").FirstOrDefault();
        }

        [TestMethod]
        public void ParseIfc4Add1()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.IFC4_ADD1, SchemaSources.IFC4_ADD1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseIfc4Add2()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.IFC4_ADD2, SchemaSources.IFC4_ADD2);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseIfc4Add2WithAlignment()
        {
            var result = SchemaModel.LoadIfc4Add2WithAlignmentExtension();
            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void ParseCis2()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.CIS2_lpm61, SchemaSources.CIS2);
            var lastError = parser.Errors.LastOrDefault();
            if (lastError != null)
                Debug.WriteLine(lastError);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseCobie()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.COBieExpress, SchemaSources.COBIE);
            var lastError = parser.Errors.LastOrDefault();
            if (lastError != null)
                Debug.WriteLine(lastError);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseStep42()
        {
            var parser = new ExpressParser();
            var data = GetFullStepDefinitions();
            var result = parser.Parse(data, SchemaSources.StepGeometry);
            var lastError = parser.Errors.LastOrDefault();
            if (lastError != null)
                Debug.WriteLine(lastError);
            Assert.IsTrue(result);
        }

        private static string GetFullStepDefinitions()
        {
            var result = "";
            result += Schemas.Step42_geometry_schema;
                result += Schemas.Step43_representation_schema;
                result += Schemas.Step41_application_context_schema;
                result += Schemas.Step49_method_definition_schema;
                result += Schemas.Step45_material_property_definition_schema;
                result += Schemas.Step44_product_structure_schema;
                result += Schemas.Step50_mathematical_functions_schema;
                result += Schemas.ISO13584_generic_expressions_schema;
            return result;
        }
    }
}
