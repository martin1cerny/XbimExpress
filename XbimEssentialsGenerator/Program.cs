using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xbim.CodeGeneration;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace XbimEssentialsGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();
            //set working directory if specified.
            if (args.Length > 0) Environment.CurrentDirectory = args[0];

            //prepare all schemas
            var ifc2X3 = SchemaModel.LoadIfc2x3();
            var ifc2X3Domains = DomainStructure.LoadIfc2X3();
            var ifc4Domains = DomainStructure.LoadIfc4Add1();
            SetTypeNumbersForIfc2X3(ifc2X3);
            var ifc4 = SchemaModel.LoadIfc4Add1();
            var cobie = SchemaModel.Load(Schemas.COBieExpress);
            //Change names to prevent name clashes
            foreach (var entity in cobie.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;

            //enhancements
            EnhanceNullStyleInIfc(ifc2X3, ifc2X3Domains);
            EnhanceNullStyleInIfc(ifc4, ifc4Domains);

            var settings = new GeneratorSettings
            {
                Structure = ifc2X3Domains,
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Common",
                IsIndexedEntity = e => IndexedClassesIfc2X3.Contains(e.Name),
                GenerateAllAsInterfaces = true,
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


            settings.IsIndexedEntity = null;
            settings.Structure = null;
            settings.OutputPath = "Xbim.CobieExpress";
            Generator.GenerateSchema(settings, cobie);
            Console.WriteLine(@"COBieExpress generated");

            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds/1000);
            Console.Beep(440, 1000);
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

        private static void SetTypeNumbersForIfc2X3(SchemaModel model)
        {
            var max = -1;
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
                    Console.WriteLine(@"Type not found: " + name);
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
        }

        private static readonly List<string> IndexedClassesIfc2X3 = new List<string> { "IfcAddress", "IfcOrganizationRelationship", "IfcApproval", "IfcApprovalActorRelationship", "IfcApprovalPropertyRelationship", "IfcApprovalRelationship", "IfcResourceApprovalRelationship", "IfcRoot", "IfcConstraint", "IfcConstraintAggregationRelationship", "IfcConstraintClassificationRelationship", "IfcConstraintRelationship", "IfcPropertyConstraintRelationship", "IfcAppliedValue", "IfcAppliedValueRelationship", "IfcCurrencyRelationship", "IfcReferencesValueDocument", "IfcCalendarDate", "IfcConnectionCurveGeometry", "IfcConnectionPointGeometry", "IfcConnectionSurfaceGeometry", "IfcLocalPlacement", "IfcBooleanResult", "IfcSolidModel", "IfcMappedItem", "IfcMaterialProperties", "IfcMonetaryUnit", "IfcPresentationStyleAssignment", "IfcPresentationStyle", "IfcTextStyleTextModel", "IfcExternalReference", "IfcTextStyleFontModel", "IfcTextStyleForDefinedFont", "IfcBoundaryCondition", "IfcStructuralLoad", "IfcTimeSeries", "IfcPresentationLayerAssignment", "IfcClassification", "IfcClassificationItem", "IfcClassificationItemRelationship", "IfcClassificationNotation", "IfcClassificationNotationFacet", "IfcClassificationReference", "IfcDocumentElectronicFormat", "IfcDocumentInformation", "IfcDocumentInformationRelationship", "IfcDocumentReference", "IfcLibraryInformation", "IfcLibraryReference", "IfcCompositeCurve", "IfcRepresentationMap", "IfcMaterial", "IfcMaterialClassificationRelationship", "IfcMaterialLayer", "IfcMaterialLayerSet", "IfcMaterialLayerSetUsage", "IfcMaterialList", "IfcUnitAssignment", "IfcProperty", "IfcPropertyDependencyRelationship", "IfcRepresentationContext", "IfcProductDefinitionShape", "IfcRepresentation", "IfcShapeAspect", "IfcActorRole", "IfcApplication", "IfcOrganization", "IfcOwnerHistory", "IfcPerson", "IfcPersonAndOrganization", "IfcTable", "IfcTableRow" }; 
    }
}
