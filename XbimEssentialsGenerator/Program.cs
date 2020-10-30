using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace XbimEssentialsGenerator
{
    /// <summary>
    /// This is the main entry point which generates schema implementations for
    /// xBIM Essentials. It generates IFC2x3TC1, IFC4Add2 and COBieExpress which is
    /// our opinioned view on COBie after several implementation attempts.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            var watch = Stopwatch.StartNew();

            //set working directory
            Environment.CurrentDirectory = @"c:\Users\Martin\Source\Repos\XbimEssentials";

            //prepare all schemas
            var ifc2X3 = LoadIfc2x3();
            var ifc2X3Domains = DomainStructure.LoadIfc2X3();
            EnhanceNullStyleInIfc(ifc2X3, ifc2X3Domains);

            var ifc4 = LoadIfc4Add2WithAlignmentExtension();
            var ifc4Domains = DomainStructure.LoadIfc4x1();
            EnhanceNullStyleInIfc(ifc4, ifc4Domains);
            //Move enums into Interfaces namespace in IFC4
            MoveEnumsToInterfaces(ifc4Domains, ifc4, Environment.CurrentDirectory, "Xbim.Ifc4");


            var ifc4x3 = LoadIfc4x3_RC1();
            var ifc4x3Domains = DomainStructure.LoadIfc4x3();
            EnhanceNullStyleInIfc(ifc4x3, ifc4x3Domains);


            var settings = new GeneratorSettings
            {
                Structure = ifc2X3Domains,
                OutputPath = "Xbim.Ifc2x3",
                IgnoreDerivedAttributes = GetIgnoreDerivedAttributes(),
                GenerateInterfaces = false
            };
            Generator.GenerateSchema(settings, ifc2X3);
            Console.WriteLine(@"IFC2x3 without interfaces generated");

            //generate cross schema access
            settings.CrossAccessProjectPath = "Xbim.Ifc4";
            settings.CrossAccessStructure = ifc4Domains;
            settings.SchemaInterfacesNamespace = "Interfaces";
            Generator.GenerateCrossAccess(settings, ifc2X3, ifc4);
            Console.WriteLine(@"IFC4 interface access generated for IFC2x3");

            settings.Structure = ifc4Domains;
            settings.OutputPath = "Xbim.Ifc4";
            settings.GenerateInterfaces = true;
            settings.SchemaInterfacesNamespace = "Xbim.Ifc4.Interfaces";
            Generator.GenerateSchema(settings, ifc4);
            Console.WriteLine(@"IFC4 with interfaces generated");

            settings.Structure = ifc4x3Domains;
            settings.OutputPath = "Xbim.Ifc4x3";
            settings.GenerateInterfaces = false;
            Generator.GenerateSchema(settings, ifc4x3);
            Console.WriteLine(@"IFC4x3 RC1 without interfaces generated");

            //generate cross schema access
            settings.CrossAccessProjectPath = "Xbim.Ifc4";
            settings.CrossAccessStructure = ifc4Domains;
            settings.SchemaInterfacesNamespace = "Interfaces";
            Generator.GenerateCrossAccess(settings, ifc4x3, ifc4);
            Console.WriteLine(@"IFC4 interface access generated for IFC Rail");

            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds/1000);
            Console.Beep(440, 500);
            Console.ReadKey();

        }

        public static SchemaModel LoadIfc4Add2WithAlignmentExtension()
        {
            var result = "";
            result += File.ReadAllText(@"Schemas\IFC4_ADD2.exp");
            result += File.ReadAllText(@"Schemas\IfcAlignmentExtension.exp");

            return SchemaModel.Load(result, "IFC4_ADD2");
        }

        public static SchemaModel LoadIfc2x3()
        {
            return SchemaModel.Load(File.ReadAllText(@"Schemas\IFC2X3_TC1.exp"), SchemaSources.IFC2x3_TC1);
        }

        public static SchemaModel LoadIfc4x3_RC1()
        {
            return SchemaModel.Load(File.ReadAllText(@"Schemas\IFC4x3_RC1.exp"), "IFC4X3_RC1");
        }

        private static List<AttributeInfo> GetIgnoreDerivedAttributes()
        {
            return new List<AttributeInfo>
            {
                new AttributeInfo
                {
                    Name = "Dim",
                    EntityName = "IfcGeometricSetSelect"
                },
                new AttributeInfo
                {
                    Name = "P",
                    EntityName = "IfcAxis2Placement"
                },
                new AttributeInfo
                {
                    Name = "Dim",
                    EntityName = "IfcBooleanOperand"
                }
            };
        }

        private static void MoveEnumsToInterfaces(DomainStructure domains, SchemaModel model, string dir, string prj)
        {
            const string iName = "Interfaces";
            var enums = model.Get<EnumerationType>();
            var iDomain = domains.Domains.FirstOrDefault(d => d.Name == iName);
            if (iDomain == null)
            {
                iDomain = new Domain{Name = iName, Types = new List<string>()};
                domains.Domains.Add(iDomain);
            }
            foreach (var enumeration in enums)
            {
                var enumName = enumeration.PersistanceName;
                var domain = domains.GetDomainForType(enumName);
                if (domain != null)
                {
                    //remove from where it is in the documentation
                    domain.Types.Remove(enumName);
                    
                    //remove old files if generated before
                    var path = Path.Combine(dir, prj, domain.Name, enumName + ".cs");
                    if(File.Exists(path))
                        File.Delete(path);
                }
                //add to interfaces namespace
                iDomain.Types.Add(enumName);
            }
        }

        private static void EnhanceNullStyleInIfc(SchemaModel model, DomainStructure structure)
        {
            var nullStyle = model.Get<EnumerationType>(n => n.Name == "IfcNullStyle").FirstOrDefault();
            if (nullStyle == null) return;
            nullStyle.Name = "IfcNullStyleEnum";
            nullStyle.PersistanceName = "IfcNullStyleEnum";

            var defType = model.New<DefinedType>(nullStyle.ParentSchema, d =>
            {
                d.Name = "IfcNullStyle";
                d.PersistanceName = "IfcNullStyle";
                d.Domain = nullStyle;
            });

            var selects = model.Get<SelectType>(s => s.Selections.Contains(nullStyle));
            foreach (var @select in selects)
            {
                select.Selections.Remove(nullStyle);
                select.Selections.Add(defType);
            }
            
            //adjust namespace
            var domain = structure.GetDomainForType("IfcNullStyle");
            domain.Types.Add("IfcNullStyleEnum");
        }

       

     
    }
}
