using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.CodeGeneration.Settings;
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
                Namespace = "Xbim",
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "",
                ClassSettings = new EntitySettings()
            };
            var schema = SchemaModel.LoadIfc2x3();

            var generator = new Generator(settings, schema);
            generator.Generate();
        }
    }
}
