using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Xbim.ExpressParser.Schemas
{
    public class Domain
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Type")]
        public List<string> Types { get; set; }
    }

    [XmlRoot("Schema")]
    public class SchemaStructure
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Domain")]
        public List<Domain> Domains { get; set; }

        public static SchemaStructure Load(Stream structure)
        {
            var serializer = new XmlSerializer(typeof(SchemaStructure));
            return (SchemaStructure)serializer.Deserialize(structure);
        }

        public static SchemaStructure LoadIfc2X3()
        {
            var txtReader = new StringReader(SchemasStructures.IFC2x3_TC1);
            var serializer = new XmlSerializer(typeof(SchemaStructure));
            return (SchemaStructure)serializer.Deserialize(txtReader);
        }

        public static SchemaStructure LoadIfc4()
        {
            var txtReader = new StringReader(SchemasStructures.IFC4);
            var serializer = new XmlSerializer(typeof(SchemaStructure));
            return (SchemaStructure)serializer.Deserialize(txtReader);
        }

        public Domain GetDomainForType(string type)
        {
            if (Domains == null)
                return null;
            return Domains.FirstOrDefault(d => d.Types != null && d.Types.Contains(type));
        }
    }
}
