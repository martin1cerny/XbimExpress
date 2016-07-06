using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace XbimEssentialsGenerator
{
    internal class Program
    {
        private static void Main()
        {
            var watch = new Stopwatch();
            watch.Start();

            //set working directory
            Environment.CurrentDirectory = "c:\\CODE\\XbimGit\\XbimEssentials";

            //prepare all schemas
            var ifc2X3 = SchemaModel.LoadIfc2x3();
            var ifc2X3Domains = DomainStructure.LoadIfc2X3();
            EnhanceNullStyleInIfc(ifc2X3, ifc2X3Domains);
            //var max = SetTypeNumbers(ifc2X3);
            SetTypeNumbers(ifc2X3);

            var ifc4 = SchemaModel.LoadIfc4Add1();
            var ifc4Domains = DomainStructure.LoadIfc4Add1();
            EnhanceNullStyleInIfc(ifc4, ifc4Domains);
            //SetTypeNumbers(ifc4, ifc2X3, max);
            SetTypeNumbers(ifc4);

            var cobie = SchemaModel.Load(Schemas.COBieExpress);
            SetTypeNumbers(cobie);

            //Change names to prevent name clashes
            foreach (var entity in cobie.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;


            //Move enums into Interfaces namespace in IFC4
            MoveEnumsToInterfaces(ifc4Domains, ifc4, Environment.CurrentDirectory, "Xbim.Ifc4");

            var settings = new GeneratorSettings
            {
                Structure = ifc2X3Domains,
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Common",
                IgnoreDerivedAttributes = GetIgnoreDerivedAttributes()
            };
            Generator.GenerateSchema(settings, ifc2X3);
            Console.WriteLine(@"IFC2x3 with interfaces generated");

            //generate cross schema access
            settings.CrossAccessProjectPath = "Xbim.Ifc4";
            settings.CrossAccessStructure = ifc4Domains;
            Generator.GenerateCrossAccess(settings, ifc2X3, ifc4);
            Console.WriteLine(@"IFC4 interface acces generated for IFC2x3");

            settings.Structure = ifc4Domains;
            settings.OutputPath = "Xbim.Ifc4";
            Generator.GenerateSchema(settings, ifc4);
            Console.WriteLine(@"IFC4 with interfaces generated");
            
            
            settings.Structure = null;
            settings.OutputPath = "Xbim.CobieExpress";
            Generator.GenerateSchema(settings, cobie);
            Console.WriteLine(@"COBieExpress generated");

            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds/1000);
            Console.Beep(440, 500);
            Console.ReadKey();

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
            var project = Generator.GetProject(Path.Combine(dir, prj));
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

                    //remove from the project
                    var compilPath = Path.Combine(domain.Name, enumName + ".cs");
                    Generator.RemoveCompilationItem(compilPath, project);
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

        private static int SetTypeNumbers(SchemaModel model, SchemaModel dependency = null, int max = 0)
        {
            var types = model.Get<NamedType>().ToList();
            const string extension = "_TYPE_IDS.csv";


            var ids = new Dictionary<string, int>();
            var file = model.FirstSchema.Name + extension;
            var depFile = dependency != null ? dependency.FirstSchema.Name + extension : null;

            var source = depFile ?? file;
            if (File.Exists(source))
            {
                var data = File.ReadAllText(source);
                var kvps = data.Trim().Split('\n');
                foreach (var vals in kvps.Select(kvp => kvp.Split(',')))
                {
                    ids.Add(vals[0], int.Parse(vals[1]));
                }
            }

            //reset latest values
            foreach (var type in types.ToList())
            {
                int id;
                if (!ids.TryGetValue(type.PersistanceName, out id)) continue;
                type.TypeId = id;
                max = Math.Max(max, id);
                types.Remove(type);
            }

            //set new values to the new types
            foreach (var type in types)
                type.TypeId = ++max;

            using (var o = File.CreateText(file))
            {
                //save for the next processing
                foreach (var type in model.Get<NamedType>())
                {
                    o.Write("{0},{1}\n", type.PersistanceName, type.TypeId);
                }
                o.Close();
            }

            return max;
        }

        private static int SetTypeNumbersForIfc2X3(SchemaModel model, int max = -1)
        {
            var types = model.Get<NamedType>().ToList();

            var values = Enum.GetValues(typeof (IfcEntityNameEnum)).Cast<short>();
            foreach (var value in values)
            {
                var s = Enum.GetName(typeof (IfcEntityNameEnum), value);
                if (s == null) continue;
                var name = s.ToUpperInvariant();
                var type = types.FirstOrDefault(t => string.Compare(t.PersistanceName, name, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (type == null)
                {
                    Console.WriteLine(@"Type {0} not found in {1}", name, model.FirstSchema.Name);
                    continue;
                }

                type.TypeId = value;
                if (value > max) max = value;
                types.Remove(type);
            }

            //change all other type IDs so that there are no duplicates
            foreach (var type in types)
            {
                type.TypeId = ++max;
            }

            return max;
        }
    }
}
