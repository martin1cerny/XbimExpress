using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XbimSchemaComparer.Comparators
{
    public interface IComparisonResult<T>
    {
        T First { get; }
        T Second { get; }
        string Message { get; }
        CommarisonResultType ResultType { get; }
    }
}
