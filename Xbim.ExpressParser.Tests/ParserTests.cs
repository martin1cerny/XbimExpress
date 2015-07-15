using System;
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
            var result = parser.Parse(SchemasDefinitions.IFC2X3_TC1);
            Assert.IsTrue(result);


            var wall = parser.SchemaInstance.Get<EntityDefinition>(d => d.Name == "IfcWall").FirstOrDefault();
            Assert.IsNotNull(wall, "There should be a definition of IfcWall entity.");
            var wallExplicitAttrs = wall.AllExplicitAttributes.ToList();
            Assert.IsTrue(wallExplicitAttrs.Any(), "There should be some explicit attributes for a wall.");
            Assert.AreEqual(8, wallExplicitAttrs.Count, "There should be 8 explicit attributes for a wall.");
        }

        [TestMethod]
        public void ParseIfc2x4()
        {
            var parser = new ExpressParser();
            var result = parser.Parse(SchemasDefinitions.IFC4);
            Assert.IsTrue(result);
        }
    }
}
