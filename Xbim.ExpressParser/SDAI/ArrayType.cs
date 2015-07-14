using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public class ArrayType: AggregationType
    {
        public int LowerIndex { get; set; }
        public int UpperIndex { get; set; }
        public bool UniqueFlag { get; set; }
        public bool OptionalFlag { get; set; }
    }
}
