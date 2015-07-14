using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class AggregationType: SchemaEntity, BaseType
    {
        public BaseType ElementType { get; set; }
    }
}
