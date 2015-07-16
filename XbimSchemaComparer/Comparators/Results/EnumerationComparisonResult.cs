using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class EnumerationComparisonResult: NamedTypeComparisonResult<EnumerationType>
    {
        public EnumerationComparisonResult(EnumerationType o, EnumerationType n): base(o,n)
        {
            
        }
    }
}
