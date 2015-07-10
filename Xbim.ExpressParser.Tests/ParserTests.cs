using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.ExpressParser.Schemas;

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
