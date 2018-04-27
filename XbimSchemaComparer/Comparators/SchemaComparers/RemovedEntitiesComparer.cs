using System;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.Results;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class RemovedEntitiesComparer : NamedTypeAddedRemovedComparer<EntityDefinition>
    {
        public RemovedEntitiesComparer() : base(
            "Entities removed from schema",
            new Guid("2592FE24-3A33-4665-A2DA-A4376803CBBA"),
            s => s.Get<EntityDefinition>(),
            AddedRemovedEnum.Removed,
            (o, n) => new EntityComparisonResult(o, n)
            )
        {
        }
    }
}