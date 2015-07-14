using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class WhereRule: SchemaEntity
    {
        public ExpressId? Label { get; set; }
        public TypeOrRule ParentItem { get; set; }
    }
}
