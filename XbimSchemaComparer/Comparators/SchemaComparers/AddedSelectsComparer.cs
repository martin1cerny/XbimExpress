using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class AddedSelectsComparer : NamedTypeAddedRemovedComparer<SelectType>
    {
        public AddedSelectsComparer() : base(
            "Selects added to schema",
            new Guid("B820DAC7-580A-4F41-9C4A-9101EF61475D"),
            s => s.Get<SelectType>(),
            AddedRemovedEnum.Added,
            (o, n) => new SelectComparisonResult(o, n)
            )
        {
        }
    }
}