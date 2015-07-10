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

    public class Schema
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Domain")]
        public List<Domain> Domains { get; set; }

        public static Schema LoadIFC2x3()
        {
            var txtReader = new StringReader(SchemasStructures.IFC2x3_TC1);
            var serializer = new XmlSerializer(typeof(Schema));
            return (Schema)serializer.Deserialize(txtReader);
        }

        public static Schema LoadIFC4()
        {
            var txtReader = new StringReader(SchemasStructures.IFC4);
            var serializer = new XmlSerializer(typeof(Schema));
            return (Schema)serializer.Deserialize(txtReader);
        }

        public Domain GetDomainForType(string type)
        {
            if (Domains == null)
                return null;
            return Domains.FirstOrDefault(d => d.Types != null && d.Types.Contains(type));
        }
    }
}
