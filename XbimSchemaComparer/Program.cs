using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.ExpressParser;
using Xbim.ExpressParser.ExpressDefinitions;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators;
using XbimSchemaComparer.Comparators.SchemaComparers;

namespace XbimSchemaComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            var ifc2X3 = GetSchema(Schemas.IFC2X3_TC1);
            var ifc4Add1 = GetSchema(Schemas.IFC4_ADD1);
            var ifc4 = GetSchema(Schemas.IFC4);

            Compare( ifc2X3, ifc4Add1 );
            Compare( ifc2X3, ifc4 );
            Compare( ifc4, ifc4Add1);

            Console.ReadLine();
        }

        private static void Compare(SchemaModel modelA, SchemaModel modelB)
        {
            Console.WriteLine(@"Schemas to compare: {0}, {1}", modelA.FirstSchema.Name, modelB.FirstSchema.Name);
            var schemas = new List<SchemaDefinition> { modelA.FirstSchema, modelB.FirstSchema };

            var schemaComparers = new ISchemaComparer[]
            {
                new AddedEntitiesComparer(), 
                new AddedSelectsComparer() , 
                new AddedTypesComparer(),
                new AddedEnumerationsComparer(), 
                new RemovedEntitiesComparer(), 
                new RemovedSelectsComparer(), 
                new RemovedTypesComparer(),
                new RemovedEnumerationsComparer(),
                new ChangedEntitiesComparer()
            };
            foreach (var comparer in schemaComparers)
            {
                comparer.Compare(modelA.FirstSchema, modelB.FirstSchema);
            }


            var w = new StringWriter();
            w.WriteLine("Number of entities:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count());
            w.WriteLine();

            w.WriteLine("Number of non-abstract entities:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count(e => e.Instantiable));
            w.WriteLine();

            w.WriteLine("Number of types:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Types.Count());
            w.WriteLine();

            w.WriteLine("Number of enumerations:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.Enumerations.Count());
            w.WriteLine();

            w.WriteLine("Number of select types:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.SelectTypes.Count());
            w.WriteLine();

            w.WriteLine("Number of global rules:");
            foreach (var schema in schemas)
                w.WriteLine("{0}: {1}", schema.Name, schema.GlobalRules.Count());
            w.WriteLine();

            foreach (var cmp in schemaComparers)
            {
                w.WriteLine("{0} ({1}):", cmp.Name, cmp.Results.Count());
                Console.WriteLine(@"{0} ({1}):", cmp.Name, cmp.Results.Count());
                foreach (var result in cmp.Results)
                    w.WriteLine(result.Message);
                w.WriteLine();
            }

            var log = w.ToString();
            var logName = String.Format("{0}_{1}.txt", modelA.FirstSchema.Name, modelB.FirstSchema.Name);
            using (var file = File.CreateText(logName))
            {
                file.Write(log);
                file.Close();
            }
            Console.WriteLine(@"Comparison saved fo file: " + Path.Combine(Environment.CurrentDirectory, logName));
            Console.WriteLine();
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
