using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class NamedType : SchemaEntity, BaseType, TypeOrRule
    {
        public ExpressId Name { get; set; }
        public List<WhereRule> WhereRules { get; set; }
        public SchemaDefinition ParentSchema { get; set; }
    }
}
