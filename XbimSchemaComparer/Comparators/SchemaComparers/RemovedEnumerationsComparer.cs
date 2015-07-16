using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class RemovedEnumerationsComparer : NamedTypeAddedRemovedComparer<EnumerationType>
    {
        public RemovedEnumerationsComparer()
            : base(
                "Enumerations removed from schema",
                new Guid("BE8412DD-6FD2-44FE-8AEC-4B968EB57A97"),
                s => s.Enumerations,
                AddedRemovedEnum.Removed,
                (o, n) => new EnumerationComparisonResult(o, n)
                )
        {
            
        }
    }
}
