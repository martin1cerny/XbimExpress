using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xbim.CodeGeneration.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var g = new GenerateClasses();
            g.Generate();
        }
    }
}
