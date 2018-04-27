using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    internal class ChangedSelectsComparer : NamedTypeSchemaComparer<SelectType>
    {
        private readonly List<ISchemaComparer<SelectType, IComparisonResult<SelectType>>> _comparers = new List<ISchemaComparer<SelectType, IComparisonResult<SelectType>>>();

        public ChangedSelectsComparer(): base(
            "Changed selects",
            new Guid("5A2C0A21-220E-45F1-81F6-81FBAB1C9DE4"),
            s => s.Get<SelectType>()
            )
        {
            _comparers.Add(new SelectComparer());
        }

        protected override IEnumerable<ISchemaComparer<SelectType, IComparisonResult<SelectType>>> Comparers => _comparers;
    }

    internal class SelectComparer : ISchemaComparer<SelectType, IComparisonResult<SelectType>>
    {
        private List<SelectResult> _results = new List<SelectResult>();
        public Guid Id => new Guid("15088C5D-A3C0-4982-AB1F-720AC3E15EA3");

        public IEnumerable<IComparisonResult<SelectType>> ComparisonResults => _results;

        public Type ResultType => typeof(SelectType);

        public IEnumerable<IComparisonResult> Results => _results;

        public string Name => "Select members comparison";

        public IEnumerable<IComparisonResult<SelectType>> Compare(SelectType oldObject, SelectType newObject)
        {
            var oMembers = oldObject.Selections.Select(s => s.Name).ToList();
            var nMembers = newObject.Selections.Select(s => s.Name).ToList();

            var match = oMembers.Where(o => nMembers.Contains(o)).ToList();
            var added = nMembers.Where(n => !oMembers.Contains(n)).ToList();
            var removed = oMembers.Where(n => !nMembers.Contains(n)).ToList();

            var result = new SelectResult(oldObject, newObject);
            if (added.Count == 0 && removed.Count == 0)
            {
                result.ResultType = ComparisonResultType.Identical;
                return Enumerable.Empty<SelectResult>();
            }

            result.ResultType = ComparisonResultType.Changed;

            var w = new StringWriter();
            w.WriteLine($"Select type {newObject.Name}:");
            foreach (var item in added)
                w.WriteLine($"    Added select: {item}");
            foreach (var item in removed)
                w.WriteLine($"    Removed select: {item}");
            result.Message = w.ToString();

            _results.Add(result);
            return new[] { result };
        }

        public IEnumerable<IComparisonResult> Compare(object oldObject, object newObject)
        {
            if (oldObject is SelectType oldSelect && newObject is SelectType newSelect)
                return Compare(oldSelect, newSelect);

            return Enumerable.Empty<IComparisonResult>();
        }
    }

    internal class SelectResult : ComparisonResult<SelectType>
    {
        public SelectResult(SelectType oldObject, SelectType newObject) : base(oldObject, newObject)
        {
        }

        public override string OldObjectName => OldObject.PersistanceName;

        public override string NewObjectName => NewObject.PersistanceName;

        public override string Message { get => base.Message; set => base.Message = value; }
    }
}
