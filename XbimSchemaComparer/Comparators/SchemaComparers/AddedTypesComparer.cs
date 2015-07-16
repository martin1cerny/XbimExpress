using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class AddedTypesComparer : NamedTypeAddedRemovedComparer<DefinedType>
    {
        public AddedTypesComparer() : base(
            "Defined types added to schema",
            new Guid("95813F6F-0528-410C-ABD1-DA596F7F47A3"),
            s => s.Types,
            AddedRemovedEnum.Added,
            (o, n) => new DefinedTypeComparisonResult(o, n)
            )
        {
        }
    }
}