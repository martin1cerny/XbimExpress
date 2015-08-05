using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.ExpressParser.Schemas;
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
            var result = parser.Parse(Schemas.Schemas.IFC2X3_TC1);
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
            var result = parser.Parse(Schemas.Schemas.IFC4);
            Assert.IsTrue(result);

            var type =
                parser.SchemaInstance.Get<DefinedType>(t => t.Name == "IfcPropertySetDefinitionSet").FirstOrDefault();
        }

        [TestMethod]
        public void ParseIfc4Add1()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.Schemas.IFC4_ADD1);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseCis2()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.Schemas.CIS2_lpm61);
            var lastError = parser.Errors.LastOrDefault();
            if (lastError != null)
                Debug.WriteLine(lastError);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParseCobie()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(Schemas.Schemas.COBieExpress);
            var lastError = parser.Errors.LastOrDefault();
            if (lastError != null)
                Debug.WriteLine(lastError);
            Assert.IsTrue(result);
        }
    }
}
