using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    internal class ChangedEnumerationsComparer : NamedTypeSchemaComparer<EnumerationType>
    {
        private readonly List<ISchemaComparer<EnumerationType, IComparisonResult<EnumerationType>>> _comparers = new List<ISchemaComparer<EnumerationType, IComparisonResult<EnumerationType>>>();

        public ChangedEnumerationsComparer(): base(
            "Changed enumerations",
            new Guid("DFFBBE75-5494-4D92-B05E-E663CA9459CD"),
            s => s.Get<EnumerationType>()
            )
        {
            _comparers.Add(new EnumerationComparer());
        }

        protected override IEnumerable<ISchemaComparer<EnumerationType, IComparisonResult<EnumerationType>>> Comparers => _comparers;
    }

    internal class EnumerationComparer : ISchemaComparer<EnumerationType, IComparisonResult<EnumerationType>>
    {
        private List<EnumerationResult> _results = new List<EnumerationResult>();
        public Guid Id => new Guid("AB136ED7-E935-40AB-8A64-BB02ECA0B60A");

        public IEnumerable<IComparisonResult<EnumerationType>> ComparisonResults => _results;

        public Type ResultType => typeof(EnumerationType);

        public IEnumerable<IComparisonResult> Results => _results;

        public string Name => "Enumeration members comparison";

        public IEnumerable<IComparisonResult<EnumerationType>> Compare(EnumerationType oldObject, EnumerationType newObject)
        {
            var oMembers = oldObject.Elements.Select(s => s.Value).ToList();
            var nMembers = newObject.Elements.Select(s => s.Value).ToList();

            var match = oMembers.Where(o => nMembers.Contains(o)).ToList();
            var added = nMembers.Where(n => !oMembers.Contains(n)).ToList();
            var removed = oMembers.Where(n => !nMembers.Contains(n)).ToList();

            var result = new EnumerationResult(oldObject, newObject);
            if (added.Count == 0 && removed.Count == 0)
            {
                result.ResultType = ComparisonResultType.Identical;
                return Enumerable.Empty<EnumerationResult>();
            }

            result.ResultType = ComparisonResultType.Changed;

            var w = new StringWriter();
            w.WriteLine($"Enumeration {newObject.Name}:");
            foreach (var item in added)
                w.WriteLine($"    Added: {item}");
            foreach (var item in removed)
                w.WriteLine($"    Removed: {item}");
            result.Message = w.ToString();

            _results.Add(result);
            return new[] { result };
        }

        public IEnumerable<IComparisonResult> Compare(object oldObject, object newObject)
        {
            if (oldObject is EnumerationType oldSelect && newObject is EnumerationType newSelect)
                return Compare(oldSelect, newSelect);

            return Enumerable.Empty<IComparisonResult>();
        }
    }

    internal class EnumerationResult : ComparisonResult<EnumerationType>
    {
        public EnumerationResult(EnumerationType oldObject, EnumerationType newObject) : base(oldObject, newObject)
        {
        }

        public override string OldObjectName => OldObject.PersistanceName;

        public override string NewObjectName => NewObject.PersistanceName;

        public override string Message { get => base.Message; set => base.Message = value; }
    }
}
