using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Helpers
{
    public class TypeHelper
    {
        public static string GetCSType(ExplicitAttribute attribute, GeneratorSettings settings)
        {
            var domain = attribute.Domain;
            var type = GetCSType(domain, settings);

            if(attribute.OptionalFlag && (
                (domain is SimpleType && !(domain is LogicalType) && !(domain is StringType)) ||
                domain is DefinedType || domain is EnumerationType
                ))
            type += "?";
            return type;
        }

        public static string GetCSType(BaseType type, GeneratorSettings settings)
        {
            var simple = type as SimpleType;
            if (simple != null)
            {
                if (simple is BinaryType) return "byte[]";
                if (simple is BooleanType) return "bool";
                if (simple is IntegerType) return "long";
                if (simple is LogicalType) return "bool?";
                if (simple is NumberType) return "double";
                if (simple is RealType) return "double";
                if (simple is StringType) return "string";
            }

            var entity = type as EntityDefinition;
            if (entity != null && settings.GenerateAllAsInterfaces)
                return "I" + entity.Name;

            var named = type as NamedType;
            if (named != null) return named.Name;

            var aggr = type as AggregationType;
            if (aggr != null)
            {
                return String.Format("{0}<{1}>", settings.ItemSetClassName, GetCSType(aggr.ElementType, settings));
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

        public static string GetCSType(UnderlyingType type, GeneratorSettings settings)
        {
            return GetCSType(type as BaseType, settings);
        }
    }
}
