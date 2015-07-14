using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class GlobalRule:SchemaEntity, TypeOrRule
    {
        public ExpressId Name { get; set; }
        public List<EntityDefinition> Entities { get; set; }
        public List<WhereRule> WhereRules { get; set; }
        public SchemaDefinition ParentSchema { get; set; }
    }
}
