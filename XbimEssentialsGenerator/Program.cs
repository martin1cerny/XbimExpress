using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var settings = new GeneratorSettings
            {
                Structure = DomainStructure.LoadIfc2X3(),
                OutputPath = "Xbim.Ifc2x3",
                InfrastructureOutputPath = "Xbim.Common",
                IsIndexedEntity = e => _indexedClassesIfc2x3.Contains(e.Name)
            };
            var schema = SchemaModel.LoadIfc2x3();
            SetTypeNumbersForIfc2X3(schema);
            Generator.Generate(settings, schema);
            Console.WriteLine(@"IFC2x3 generated");

            settings.Structure = DomainStructure.LoadIfc4();
            settings.OutputPath = "Xbim.Ifc4";
            schema = SchemaModel.LoadIfc4();
            Generator.Generate(settings, schema);
            Console.WriteLine(@"IFC4 generated");

            settings.IsIndexedEntity = null;
            settings.Structure = null;
            settings.OutputPath = "Xbim.CobieExpress";
            schema = SchemaModel.Load(Schemas.COBieExpress);
            //Change names to prevent name clashes
            foreach (var entity in schema.Get<EntityDefinition>())
                entity.Name = "Cobie" + entity.Name;
            Generator.Generate(settings, schema);
            Console.WriteLine(@"COBieExpress generated");

            watch.Stop();
            Console.WriteLine(@"Finished in {0}s.", watch.ElapsedMilliseconds/1000);
            Console.Beep(440, 1000);
            Console.ReadKey();

        }

        private static void SetTypeNumbersForIfc2X3(SchemaModel model)
        {
            var max = -1;
            var types = model.Get<NamedType>().ToList();

            var values = Enum.GetValues(typeof (IfcEntityNameEnum)).Cast<short>();
            foreach (var value in values)
            {
                var name = Enum.GetName(typeof (IfcEntityNameEnum), value).ToUpperInvariant();
                var type = types.FirstOrDefault(t => t.PersistanceName == name);
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

        private static List<string> _indexedClassesIfc2x3 = new List<string> { "IfcAddress", "IfcOrganizationRelationship", "IfcApproval", "IfcApprovalActorRelationship", "IfcApprovalPropertyRelationship", "IfcApprovalRelationship", "IfcResourceApprovalRelationship", "IfcRoot", "IfcConstraint", "IfcConstraintAggregationRelationship", "IfcConstraintClassificationRelationship", "IfcConstraintRelationship", "IfcPropertyConstraintRelationship", "IfcAppliedValue", "IfcAppliedValueRelationship", "IfcCurrencyRelationship", "IfcReferencesValueDocument", "IfcCalendarDate", "IfcConnectionCurveGeometry", "IfcConnectionPointGeometry", "IfcConnectionSurfaceGeometry", "IfcLocalPlacement", "IfcBooleanResult", "IfcSolidModel", "IfcMappedItem", "IfcMaterialProperties", "IfcMonetaryUnit", "IfcPresentationStyleAssignment", "IfcPresentationStyle", "IfcTextStyleTextModel", "IfcExternalReference", "IfcTextStyleFontModel", "IfcTextStyleForDefinedFont", "IfcBoundaryCondition", "IfcStructuralLoad", "IfcTimeSeries", "IfcPresentationLayerAssignment", "IfcClassification", "IfcClassificationItem", "IfcClassificationItemRelationship", "IfcClassificationNotation", "IfcClassificationNotationFacet", "IfcClassificationReference", "IfcDocumentElectronicFormat", "IfcDocumentInformation", "IfcDocumentInformationRelationship", "IfcDocumentReference", "IfcLibraryInformation", "IfcLibraryReference", "IfcCompositeCurve", "IfcRepresentationMap", "IfcMaterial", "IfcMaterialClassificationRelationship", "IfcMaterialLayer", "IfcMaterialLayerSet", "IfcMaterialLayerSetUsage", "IfcMaterialList", "IfcUnitAssignment", "IfcProperty", "IfcPropertyDependencyRelationship", "IfcRepresentationContext", "IfcProductDefinitionShape", "IfcRepresentation", "IfcShapeAspect", "IfcActorRole", "IfcApplication", "IfcOrganization", "IfcOwnerHistory", "IfcPerson", "IfcPersonAndOrganization", "IfcTable", "IfcTableRow" }; 
    }
}
