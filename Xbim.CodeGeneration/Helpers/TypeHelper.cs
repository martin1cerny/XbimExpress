using System;
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
            if (attribute.OptionalFlag && domain is AggregationType)
                type = "Optional" + type;

            return type;
        }

        public static string GetCSType(BaseType type, GeneratorSettings settings, bool useList = false)
        {
            var simple = type as SimpleType;
            if (simple != null)
            {
                if (simple is BinaryType) return "long";
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
                return useList ? 
                    string.Format("List<{0}>", GetCSType(aggr.ElementType, settings))
                    : string.Format("{0}<{1}>", settings.ItemSetClassName, GetCSType(aggr.ElementType, settings));
            }

            //this shouldn't happen
            throw new NotSupportedException();
        }

        public static string GetCSType(UnderlyingType type, GeneratorSettings settings)
        {
            while (true)
            {
                var nestedType = type as DefinedType;
                if (nestedType == null) return GetCSType(type as BaseType, settings, true);
                type = nestedType.Domain;
            }
        }
    }
}
