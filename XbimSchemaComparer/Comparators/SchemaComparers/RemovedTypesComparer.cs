using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class RemovedTypesComparer : NamedTypeAddedRemovedComparer<DefinedType>
    {
        public RemovedTypesComparer() : base(
            "Defined types removed from schema",
            new Guid("036AB9BE-A331-4296-B673-05F8094B10FE"),
            s => s.Types,
            AddedRemovedEnum.Removed,
            (o, n) => new DefinedTypeComparisonResult(o, n)
            )
        {
        }
    }
}