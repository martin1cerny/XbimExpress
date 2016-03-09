using System.Xml.Serialization;

namespace XbimValidationGenerator.Schema
{
    public class WhereRule
    {
        [XmlAttribute]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Definition { get; set; }
    }
}
