using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

// ReSharper disable once CheckNamespace
namespace Xbim.ExpressParser.SDAI
{
    public static class ExpressIdExtensions
    {
        public static void AddRange(this ICollection<ExpressId> collection, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }
    }
}
