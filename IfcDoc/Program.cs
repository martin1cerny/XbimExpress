using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace IfcDoc
{
    class Program
    {
        static void Main(string[] args)
        {

            //prepare schema
            var ifc2X3 = SchemaModel.Load(File.ReadAllText("IfcDocSchema.exp"), "IFCDOC_10_7");
            
            //set working directory
            var dir = @"c:\Users\Martin\Source\XbimIfcDoc";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Environment.CurrentDirectory = dir;

            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.IfcDoc",
                InfrastructureOutputPath = "Xbim.Common",
            };

            Generator.GenerateSchema(settings, ifc2X3);
            Console.WriteLine(@"IfcDoc generated");
            Console.Beep(440, 500);
            Console.ReadKey();

        }
    }
}
