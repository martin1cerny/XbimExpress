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
        public static string GetCSType(ExplicitAttribute attribute, string setName)
        {
            var domain = attribute.Domain;
            var type = GetCSType(domain, setName);

            if(attribute.OptionalFlag && (
                (domain is SimpleType && !(domain is LogicalType) && !(domain is StringType)) ||
                domain is DefinedType
                ))
            type += "?";
            return type;
        }

        public static string GetCSType(BaseType type, string setName)
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
                return String.Format("{0}<{1}>", setName, GetCSType(aggr.ElementType, setName));
                //if (aggr is ArrayType)
                //    return String.Format("{0}<{1}>",setName, GetCSType(aggr.ElementType, setName));
                //if (aggr is BagType)
                //    return String.Format("{0}<{1}>", setName, GetCSType(aggr.ElementType, setName));
                //if (aggr is ListType)
                //    return String.Format("{0}<{1}>", setName, GetCSType(aggr.ElementType, setName));
                //if (aggr is SetType)
                //    return String.Format("{0}<{1}>", setName, GetCSType(aggr.ElementType, setName));

            }

            //this shouldn't happen
            throw new NotSupportedException();
        }

        public static string GetCSType(UnderlyingType type, string setName)
        {
            return GetCSType(type as BaseType, setName);
        }
    }
}
