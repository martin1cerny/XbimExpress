using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class Attribute: SchemaEntity
    {
        public ExpressId Name { get; set; }
        public EntityDefinition ParentEntity { get; set; }
    }
}
