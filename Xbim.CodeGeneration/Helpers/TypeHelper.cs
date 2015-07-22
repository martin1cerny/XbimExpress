using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Helpers
{
    public class TypeHelper
    {
        public static string GetCSType(BaseType type)
        {
            var simple = type as SimpleType;
            if (simple != null)
            {
                if (simple is BinaryType) return "byte[]";
                if (simple is BooleanType) return "bool";
                if (simple is IntegerType) return "int";
                if (simple is LogicalType) return "bool?";
                if (simple is NumberType) return "float";
                if (simple is RealType) return "float";
                if (simple is StringType) return "string";
            }

            var named = type as NamedType;
            if (named != null) return named.Name;

            var aggr = type as AggregationType;
            if (aggr != null)
            {
                if (aggr is ArrayType)
                    return String.Format("{0}[]", GetCSType(aggr.ElementType));
                if (aggr is BagType)
                    return String.Format("List<{0}>", GetCSType(aggr.ElementType));
                if (aggr is ListType)
                    return String.Format("List<{0}>", GetCSType(aggr.ElementType));
                if (aggr is SetType)
                    return String.Format("HashSet<{0}>", GetCSType(aggr.ElementType));

            }

            //this shouldn't happen
            throw new NotSupportedException();
        }

        public static string GetCSType(UnderlyingType type)
        {
            return GetCSType(type as BaseType);
        }
    }
}
