using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.ExpressParser.SDAI;

namespace XbimSchemaComparer.Comparators.SchemaComparers
{
    public abstract class NamedTypeAddedRemovedComparer<TResult> : ISchemaComparer<SchemaModel, IComparisonResult<TResult>> where TResult : NamedType
    {
        private readonly List<IComparisonResult<TResult>> _results = new List<IComparisonResult<TResult>>();
        private readonly Func<SchemaModel, IEnumerable<TResult>> _accessor;
        private readonly AddedRemovedEnum _type;
        private readonly Func<TResult, TResult, IComparisonResult<TResult>> _resultCreator;

        protected NamedTypeAddedRemovedComparer(
            string name, 
            Guid id,
            Func<SchemaModel, IEnumerable<TResult>> accessor, 
            AddedRemovedEnum type,
            Func<TResult, TResult, IComparisonResult<TResult>> resultCreator)
        {
            Name = name;
            Id = id;
            _accessor = accessor;
            _type = type;
            _resultCreator = resultCreator;
        }

        public string Name
        {
            get; private set;
        }

        public Guid Id
        {
            get; private set; 
        }

        public IEnumerable<IComparisonResult<TResult>> Compare(SchemaModel oldObject, SchemaModel newObject)
        {
            List<TResult> changed;
            var results = new List<IComparisonResult<TResult>>();

            switch (_type)
            {
                case AddedRemovedEnum.Added:
                    changed =
                        _accessor(newObject).Where(oo => _accessor(oldObject).All(no => no.Name != oo.Name)).ToList();
                    break;
                case AddedRemovedEnum.Removed:
                    changed =
                        _accessor(oldObject).Where(oo => _accessor(newObject).All(no => no.Name != oo.Name)).ToList(); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!changed.Any())
                return results;

            

            var typeName = typeof (TResult).Name;
            foreach (var namedType in changed)
            {
                IComparisonResult<TResult> result;
                switch (_type)
                {
                    case AddedRemovedEnum.Added:
                        result = _resultCreator(null, namedType);
                        result.ResultType = ComparisonResultType.Added;
                        result.Message = String.Format("Added {0}: {1}", typeName, namedType.Name);
                        break;
                    case AddedRemovedEnum.Removed:
                        result = _resultCreator(namedType, null);
                        result.ResultType = ComparisonResultType.Removed;
                        result.Message = String.Format("Removed {0}: {1}", typeName, namedType.Name);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                results.Add(result);
            }

            //add to the global list which holds all runs of comparison
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

    public enum AddedRemovedEnum
    {
        Added, 
        Removed
    }
}
