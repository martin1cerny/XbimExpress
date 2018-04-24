using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Xbim.IfcDomains
{
    public class Domain
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Type")]
        public List<string> Types { get; set; }
    }

    [XmlRoot("Schema")]
    public class DomainStructure
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Domain")]
        public List<Domain> Domains { get; set; }

        public static DomainStructure Load(Stream structure)
        {
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(structure);
        }

        public static DomainStructure LoadIfc2X3()
        {
            var txtReader = new StringReader(DomainsData.IFC2x3_TC1);
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(txtReader);
        }

        public static DomainStructure LoadIfc4()
        {
            var txtReader = new StringReader(DomainsData.IFC4);
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(txtReader);
        }

        public static DomainStructure LoadIfc4Add1()
        {
            var txtReader = new StringReader(DomainsData.IFC4Add1);
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(txtReader);
        }

        public static DomainStructure LoadIfc4Add2()
        {
            var txtReader = new StringReader(DomainsData.IFC4Add2);
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(txtReader);
        }

        public static DomainStructure LoadIfc4x1()
        {
            var txtReader = new StringReader(DomainsData.IFC4x1);
            var serializer = new XmlSerializer(typeof(DomainStructure));
            return (DomainStructure)serializer.Deserialize(txtReader);
        }

        public Domain GetDomainForType(string type)
        {
            if (Domains == null)
                return null;
            return Domains.FirstOrDefault(d => d.Types != null && d.Types.Contains(type));
        }
    }
}
