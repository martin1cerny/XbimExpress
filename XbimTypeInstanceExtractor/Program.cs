using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Xbim.ExpressParser.SDAI;
using XbimValidationGenerator.Schema;
using WhereRule = XbimValidationGenerator.Schema.WhereRule;

namespace XbimTypeInstanceExtractor
{
    /// <summary>
    /// This utility is used to create reports of types and type hierarchies
    /// from IFC2x3 and IFC4 starting from IfcElement and IfcElementType.
    /// These results can be used in loosely coupled systems to mimic IFC
    /// hierarchical structure.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            var schema = SchemaModel.LoadIfc4Add1();
            using (var w = File.CreateText("ElementTypesIFC4.csv"))
            {
                WriteElementTypes(schema, w);
                w.Close();
            }

            using (var w = File.CreateText("ElementTypesHierarchyIFC4.csv"))
            {
                WriteElementTypesHierarchy(schema, w);
                w.Close();
            }

            schema = SchemaModel.LoadIfc2x3();
            using (var w = File.CreateText("ElementTypesIFC2x3.csv"))
            {
                WriteElementTypes(schema, w);
                w.Close();
            }

            using (var w = File.CreateText("ElementTypesHierarchyIFC2x3.csv"))
            {
                WriteElementTypesHierarchy(schema, w);
                w.Close();
            }
        }



        private static void WriteElementTypesHierarchy(SchemaModel schema, TextWriter w)
        {
            var elementType = schema.Get<EntityDefinition>(e => e.Name == "IfcElementType").FirstOrDefault();
            if (elementType == null)
                throw new Exception();
            w.WriteLine("IfcElementType,");
            WriteHierarchy(elementType, w);
        }

        private static void WriteElementTypes(SchemaModel schema, TextWriter w)
        {
            var element = schema.Get<EntityDefinition>(e => e.Name == "IfcElement").FirstOrDefault();
            if (element == null)
                throw new Exception();

            var elements = element.AllSubTypes.ToList();
            elements.Add(element);
            foreach (var definition in elements)
            {
                //try to get just from name (IfcWall - IfcWallType)
                var type = GetCorrespondingType(definition);
                if (type != null)
                {
                    w.WriteLine("{0},{1},{2}", definition.Name, definition.Instantiable ? "Concrete" : "Abstract", type);
                    continue;
                }
                //no type match found
                w.WriteLine("{0},{1},{2}", definition.Name, definition.Instantiable ? "Concrete" : "Abstract", "no-type");
            }
        }

        private static void WriteHierarchy(EntityDefinition entity, TextWriter writer)
        {
            foreach (var definition in entity.SubTypes)
            {
                writer.WriteLine("{0},{1},{2}", definition.Name, entity.Name, entity.Instantiable);
                WriteHierarchy(definition, writer);
            }
        }

        private static WhereRule GetTypeAssignedRule(EntityDefinition entity, SchemaRules rules)
        {
            const string ruleName = "CorrectTypeAssigned";
            while (true)
            {
                if (entity == null)
                    return null;
                var rule = entity.WhereRules.FirstOrDefault(r => r.Label == ruleName);
                if (rule == null && entity.PersistanceName == "IfcElement")
                    return null;
                if (rule == null)
                {
                    entity = entity.Supertypes.FirstOrDefault();
                    continue;
                }

                var typeRules = rules.TypeRulesSet.FirstOrDefault(r => r.Type == entity.PersistanceName);
                return typeRules != null ? typeRules.WhereRules.FirstOrDefault(r => r.Name == ruleName) : null;
            }
        }

        private static string GetElementType(WhereRule rule)
        {
            return new Regex("Ifc\\w+Type", RegexOptions.Compiled).Match(rule.Description).Value;
        }

        private static string GetCorrespondingType(EntityDefinition entity)
        {
            while (true)
            {
                if (entity == null)
                    return null;
                var schema = entity.SchemaModel;
                var typeName = entity.PersistanceName + "Type";
                var type = schema.Get<EntityDefinition>(e => e.PersistanceName == typeName).FirstOrDefault();
                if (type != null)
                    return typeName;

                typeName = entity.PersistanceName + "Style";
                type = schema.Get<EntityDefinition>(e => e.PersistanceName == typeName).FirstOrDefault();
                if (type != null)
                    return typeName;


                entity = entity.Supertypes.FirstOrDefault();
                if (entity.PersistanceName == "IfcElement")
                    return null;
            }
        }
    }
}
