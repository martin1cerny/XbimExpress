using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser;
using Xbim.ExpressParser.Schemas;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

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

            var w = new StringWriter();
            w.WriteLine("Number of entities:");
            foreach (var schema in schemas)
            {
                w.WriteLine("{0}: {1}", schema.Name, schema.Entities.Count());
            }
            w.WriteLine("New entities in {0}:", ifc4.Schema.Name);
            foreach (var entity in ifc4.Schema.Entities.Where(e => ifc2X3.Schema.Entities.All(et => et.Name != e.Name)))
            {
                w.WriteLine("{0} ({1})", entity.Name, ifc4Domain.GetDomainForType(entity.Name).Name);
            }
            w.WriteLine();
            w.WriteLine("Removed entities in {0}:", ifc4.Schema.Name);
            foreach (var entity in ifc2X3.Schema.Entities.Where(e => ifc4.Schema.Entities.All(et => et.Name != e.Name)))
            {
                w.WriteLine("{0} ({1})", entity.Name, ifc2X3Domain.GetDomainForType(entity.Name).Name);
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

            Console.WriteLine(w.ToString());
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
