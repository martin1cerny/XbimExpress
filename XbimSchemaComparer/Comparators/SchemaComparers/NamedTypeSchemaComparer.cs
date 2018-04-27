using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public abstract class NamedTypeSchemaComparer<TResult> : ISchemaComparer<SchemaModel, IComparisonResult<TResult>> 
        where TResult : NamedType
    {
        private readonly List<IComparisonResult<TResult>> _results = new List<IComparisonResult<TResult>>();
        private readonly Func<SchemaModel, IEnumerable<TResult>> _accessor;

        protected NamedTypeSchemaComparer(
            string name, 
            Guid id,
            Func<SchemaModel, IEnumerable<TResult>> accessor)
        {
            Name = name;
            Id = id;
            _accessor = accessor;
        }

        public string Name
        {
            get; private set;
        }

        public Guid Id
        {
            get; private set; 
        }

        protected abstract IEnumerable<ISchemaComparer<TResult, IComparisonResult<TResult>>> Comparers { get; } 

        public IEnumerable<IComparisonResult<TResult>> Compare(SchemaModel oldObject, SchemaModel newObject)
        {
            var results = new List<IComparisonResult<TResult>>();
            
            //create candidate pairs
            var o = _accessor(oldObject).ToList();
            var n = _accessor(newObject).ToList();

            var candidates = (from oo in o let ooName = oo.Name let no = n.FirstOrDefault(c => c.Name == ooName) 
                              where no != null 
                              select new Tuple<TResult, TResult>(oo, no)).ToList();

            //use all comparers for all tuples
            foreach (var comparer in Comparers)
                foreach (var candidate in candidates)
                    results.AddRange(comparer.Compare(candidate.Item1, candidate.Item2)); 


            _results.AddRange(results);
            return results;
        }

        public IEnumerable<IComparisonResult> Results
        {
            get { return _results; }
        }

        public IEnumerable<IComparisonResult<TResult>> ComparisonResults
        {
            get { return _results; }
        }

        public Type ResultType
        {
            get { return typeof(TResult); }
        }

        IEnumerable<IComparisonResult> ISchemaComparer.Compare(object oldObject, object newObject)
        {
            return Compare(oldObject as SchemaModel, newObject as SchemaModel);
        }
    }
}
