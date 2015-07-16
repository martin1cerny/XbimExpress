using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XbimSchemaComparer.Comparators
{
    public interface IComparisonResult
    {
        object OldObject { get; }
        object NewObject { get; }
        string Message { get; set; }
        ComparisonResultType ResultType { get; set; }
        string OldObjectName { get; }
        string NewObjectName { get; }
    }

    public interface IComparisonResult<out T> : IComparisonResult
    {
        
    }

    public abstract class ComparisonResult<T> : IComparisonResult<T>
    {
        protected ComparisonResult(T oldObject, T newObject)
        {
            OldObject = oldObject;
            NewObject = newObject;

            //initialize result defaults
            Message = "";
            ResultType = ComparisonResultType.Identical;
        }

        public T OldObject { get; private set; }

        public T NewObject { get; private set; }

        public string Message { get; set; }

        public ComparisonResultType ResultType { get; set; }


        public abstract string OldObjectName { get; }

        public abstract string NewObjectName { get; }

        object IComparisonResult.OldObject
        {
            get { return OldObject; }
        }

        object IComparisonResult.NewObject
        {
            get { return NewObject; }
        }
    }
}