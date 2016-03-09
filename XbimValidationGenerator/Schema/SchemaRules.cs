using System.Collections.Generic;
using System.Xml.Serialization;

namespace XbimValidationGenerator.Schema
{
    public class SchemaRules
    {
        [XmlAttribute]
        public string Schema { get; set; }
        public List<TypeRules> TypeRulesSet { get; set; }
    }
}
