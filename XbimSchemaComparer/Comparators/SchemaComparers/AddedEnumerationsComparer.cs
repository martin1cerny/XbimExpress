using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class AddedEnumerationsComparer : NamedTypeAddedRemovedComparer<EnumerationType>
    {
        public AddedEnumerationsComparer()
            : base(
                "Enumerations added to schema",
                new Guid("6E12F539-941A-4D2C-9F87-52557F695C9F"),
                s => s.Enumerations,
                AddedRemovedEnum.Added,
                (o, n) => new EnumerationComparisonResult(o, n)
                )
        {
            
        }
    }
}
