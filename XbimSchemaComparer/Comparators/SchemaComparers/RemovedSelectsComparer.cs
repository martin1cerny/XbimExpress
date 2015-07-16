using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class RemovedSelectsComparer : NamedTypeAddedRemovedComparer<SelectType>
    {
        public RemovedSelectsComparer() : base(
            "Selects removed from schema",
            new Guid("E5ADBF58-32B6-405E-95FC-4DE81A0000B4"),
            s => s.SelectTypes,
            AddedRemovedEnum.Removed,
            (o, n) => new SelectComparisonResult(o, n)
            )
        {
        }
    }
}