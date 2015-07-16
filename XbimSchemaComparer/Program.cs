using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.ExpressParser;
using Xbim.ExpressParser.Schemas;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;
using XbimSchemaComparer.Comparators;
using XbimSchemaComparer.Comparators.SchemaComparers;

namespace XbimSchemaComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ifc2X3 = GetSchema(Schemas.IFC2X3_TC1);
            var ifc4 = GetSchema(Schemas.IFC4);
            var schemas = new List<SchemaDefinition> {ifc2X3.Schema, ifc4.Schema};
            var ifc2X3Domain = DomainStructure.LoadIfc2X3();
            var ifc4Domain = DomainStructure.LoadIfc4();


            var schemaComparers = new ISchemaComparer[]
            {
                new AddedEntitiesComparer(), 
                new AddedSelectsComparer() , 
                new AddedTypesComparer(),
                new AddedEnumerationsComparer(), 
                new RemovedEntitiesComparer(), 
                new RemovedSelectsComparer(), 
                new RemovedTypesComparer(),
                new RemovedEnumerationsComparer()
            };
            foreach (var comparer in schemaComparers)
            {
                comparer.Compare(ifc2X3.Schema, ifc4.Schema);
            }
            

            var w = new StringWriter();
            w.WriteLine("Number of entities:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count());
            }
            w.WriteLine();
           
            w.WriteLine("Number of types:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.Types.Count());
            }
            w.WriteLine();

            w.WriteLine("Number of enumerations:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.Enumerations.Count());
            }
            w.WriteLine();

            w.WriteLine("Number of select types:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.SelectTypes.Count());
            }
            w.WriteLine();

            w.WriteLine("Number of global rules:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.GlobalRules.Count());
            }
            w.WriteLine();

            foreach (var cmp in schemaComparers)
            {
                foreach (var result in cmp.Results)
                {
                    w.WriteLine(result.Message);
                }
                w.WriteLine();
            }

            var log = w.ToString();
            using (var file = File.CreateText("log.txt"))
            {
                file.Write(log);
                file.Close();
            }
            Console.WriteLine(log);
            Console.ReadLine();
        }

        private static SchemaModel GetSchema(string data)
        {
            var parser = new ExpressParser();
            var result = parser.Parse(data);
            if (!result)
                throw new Exception("Error parsing schema file");
            return parser.SchemaInstance;
        }
    }
}
