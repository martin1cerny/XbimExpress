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
    /// <summary>
    /// This utility is used to generate schema implementation for IFCDOC. It can than be used to
    /// read and edit IFC documentation generator. That can be useful for translations as well as
    /// for analytical task on top of IFC schema. The EXPRESS schema used for this generation is
    /// unofficial and was created using reverse engineering.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            //prepare schema
            var ifcDoc = SchemaModel.Load(File.ReadAllText("IfcDocSchema.exp"), "IFCDOC_10_7");
            
            //set working directory
            var dir = @"c:\Users\Martin\Source\XbimIfcDoc";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Environment.CurrentDirectory = dir;

            var settings = new GeneratorSettings
            {
                OutputPath = "Xbim.IfcDoc"
            };

            Generator.GenerateSchema(settings, ifcDoc);
            Console.WriteLine(@"IfcDoc generated");
            Console.Beep(440, 500);
            Console.ReadKey();

        }
    }
}
