using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class SelectComparisonResult: NamedTypeComparisonResult<SelectType>
    {
        public SelectComparisonResult(SelectType o, SelectType n): base(o,n)
        {
            
        }
    }
}
