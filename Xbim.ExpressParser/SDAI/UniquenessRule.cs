using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class UniquenessRule: SchemaEntity
    {
        public ExpressId? Label { get; set; }
        public List<Attribute> Attributes { get; set; }
        public EntityDefinition ParentEntity { get; set; }

    }
}
