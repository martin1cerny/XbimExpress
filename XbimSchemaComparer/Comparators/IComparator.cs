using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XbimSchemaComparer.Comparators
{
    interface IComparator<TObject, TRepository>
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
        /// <param name="objToCompare">Object to compare</param>
        /// <returns></returns>
        IComparisonResult<TObject> Compare(TObject objToCompare);

        /// <summary>
        /// All comparison results of this comparator
        /// </summary>
        IEnumerable<IComparisonResult<TObject>> Results { get; } 

    }
}
