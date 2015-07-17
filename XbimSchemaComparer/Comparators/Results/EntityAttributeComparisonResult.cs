using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.Results
{
    public class EntityAttributeComparisonResult : EntityComparisonResult
    {
        public EntityAttributeComparisonResult(EntityDefinition o, EntityDefinition n) : base(o, n)
        {
            Differences = new List<AttributeDifference>();
        }

        public override string Message
        {
            get
            {
                var msg = Differences.Where(d => d.ResultType == ComparisonResultType.Added)
                    .Aggregate(String.Format("Entity: {0}\n", NewObject.Name),
                        (current, difference) =>
                            current + String.Format("Added attribute: {0} \n", difference.Attribute.Name));
                msg = Differences.Where(d => d.ResultType == ComparisonResultType.Removed)
                    .Aggregate(msg,
                        (current, difference) =>
                            current + String.Format("Removed attribute: {0} \n", difference.Attribute.Name));
                return Differences.Where(d => d.ResultType == ComparisonResultType.Changed)
                    .Aggregate(msg,
                        (current, difference) =>
                            current +
                            String.Format("Moved attribute: {0} (Old index: {1}, New index: {2})\n",
                                difference.Attribute.Name, difference.OldIndex, difference.NewIndex));
            }
            set { }
        }

        public List<AttributeDifference> Differences { get; private set; }

        public int NumberOfDifferences
        {
            get { return Differences.Count; }
        }
    }

    public class AttributeDifference
    {
        public ExplicitAttribute Attribute { get; set; }
        public ComparisonResultType ResultType { get; set; }
        public int OldIndex { get; set; }
        public int NewIndex { get; set; }
    }
}