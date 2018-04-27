using System;
using System.Collections.Generic;
using Xbim.ExpressParser.SDAI;
using XbimSchemaComparer.Comparators.EntityComparers;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public class ChangedEntitiesComparer: NamedTypeSchemaComparer<EntityDefinition>
    {

        private readonly List<ISchemaComparer<EntityDefinition, IComparisonResult<EntityDefinition>>> _comparers =new List<ISchemaComparer<EntityDefinition, IComparisonResult<EntityDefinition>>>();
        public ChangedEntitiesComparer(): base(
            "Changed entity definitions",
            new Guid("6445EE3E-263B-47CC-AC7B-F7695AB7BAE3"),
            s => s.Get<EntityDefinition>()
            )
        {
            _comparers.Add(new AttributeComparer());
            _comparers.Add(new DomainComparer());
        }
        protected override IEnumerable<ISchemaComparer<EntityDefinition, IComparisonResult<EntityDefinition>>> Comparers
        {
            get { return _comparers; }
        }
    }
}
