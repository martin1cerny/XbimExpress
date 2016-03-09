using System.Collections.Generic;
using System.Xml.Serialization;

namespace XbimValidationGenerator.Schema
{
    public class TypeRules
    {
        [XmlAttribute]
        public string Type { get; set; }
        public List<WhereRule> WhereRules { get; set; }
    }
}
