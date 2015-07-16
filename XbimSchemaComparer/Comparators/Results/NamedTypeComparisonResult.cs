using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public abstract class NamedTypeComparisonResult<T>: ComparisonResult<T> where T:NamedType
    {
        protected NamedTypeComparisonResult(T oldObject, T newObject): base(oldObject, newObject)
        {
            
        }
        public override string OldObjectName
        {
            get { return OldObject != null? OldObject.Name : null; }
        }

        public override string NewObjectName
        {
            get { return NewObject != null ? NewObject.Name : null; }
        }
    }
}
