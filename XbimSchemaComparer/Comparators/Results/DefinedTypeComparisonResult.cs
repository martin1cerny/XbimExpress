using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class DefinedTypeComparisonResult: NamedTypeComparisonResult<DefinedType>
    {
        public DefinedTypeComparisonResult(DefinedType o, DefinedType n): base(o,n)
        {
            
        }
    }
}
