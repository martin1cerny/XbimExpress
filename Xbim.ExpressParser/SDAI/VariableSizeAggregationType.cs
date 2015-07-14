using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.ExpressParser.SDAI
{
    public abstract class VariableSizeAggregationType: AggregationType
    {
        public int LowerBound { get; set; }
        public int? UpperBound { get; set; }
    }
}
