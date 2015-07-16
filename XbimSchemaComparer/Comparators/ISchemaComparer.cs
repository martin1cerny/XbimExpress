using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XbimSchemaComparer.Comparators
{
    public interface ISchemaComparer
    {
        IEnumerable<IComparisonResult> Compare(object oldObject, object newObject);
        IEnumerable<IComparisonResult> Results { get; }
    }
    public interface ISchemaComparer<in TObject, out TResult> : ISchemaComparer where TResult : IComparisonResult
    {
        /// <summary>
        /// Name of the comparator should indocate which aspect this comparator compares.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Id has to be unique and might be used in the future in automated deployments
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// This function should look up the corresponding object in repository and compare it.
        /// </summary>
        /// <param name="oldObject">Object to compare</param>
        /// <param name="newObject">Object to compare</param>
        /// <returns></returns>
        IEnumerable<TResult> Compare(TObject oldObject, TObject newObject);

        /// <summary>
        /// All comparison results of this comparator
        /// </summary>
        IEnumerable<TResult> ComparisonResults { get; }

        /// <summary>
        /// This property should return TResult
        /// </summary>
        Type ResultType { get; }

    }
}
