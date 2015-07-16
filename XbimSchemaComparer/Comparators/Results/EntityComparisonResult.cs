using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class EntityComparisonResult: NamedTypeComparisonResult<EntityDefinition>
    {
        public EntityComparisonResult(EntityDefinition o, EntityDefinition n): base(o,n)
        {
            
        }
    }
}
