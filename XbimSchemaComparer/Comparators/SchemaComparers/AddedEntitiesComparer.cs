using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class AddedEntitiesComparer : NamedTypeAddedRemovedComparer<EntityDefinition>
    {
        public AddedEntitiesComparer() : base(
            "Entities added to schema",
            new Guid("2B422591-1D9C-494F-AD4F-7E7429BBC2F4"),
            s => s.Get<EntityDefinition>(),
            AddedRemovedEnum.Added,
            (o, n) => new EntityComparisonResult(o, n)
            )
        {
        }
    }
}