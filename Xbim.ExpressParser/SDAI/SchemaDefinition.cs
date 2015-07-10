using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class SchemaDefinition
    {
        public string Name { get; set; }
        public string Identification { get; set; }
        public List<EntityDefinition> Entities { get; set; }
        public List<DefinedType> Types { get; set; }
        public List<GlobalRule> GlobalRules { get; set; }

    }
}
