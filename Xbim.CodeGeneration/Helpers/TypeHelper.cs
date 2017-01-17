using System;
using System.Linq;
using Xbim.CodeGeneration.Differences;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;

namespace Xbim.CodeGeneration.Helpers
{
    public class TypeHelper
    {
        private static readonly ReadOnlyItemSets ReadOnlyItemSets = new ReadOnlyItemSets(); 

        public static string GetInterfaceCSType(ExplicitAttribute attribute, GeneratorSettings settings, string fullNamespace = null)
        {
            var domain = attribute.Domain;
            var type = GetCSType(domain, settings, false, true, fullNamespace);

            if (attribute.OptionalFlag && (
                (domain is SimpleType && !(domain is LogicalType) && !(domain is StringType)) ||
                domain is DefinedType || domain is EnumerationType
                ))
                type += "?";
            if (domain is AggregationType)
            {
                if (ReadOnlyItemSets.Any(s => s.Attribute == attribute.Name && s.Class == attribute.ParentEntity.Name))
                {
                    //replace item set with IEnumerable
                    type = type.Substring(type.IndexOf('<'));
                    type = "IEnumerable" + type;
                }
            }

            return type;
        }

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
                type = "IOptional" + type.Substring(1);

            return type;
        }

        public static string GetCSType(BaseType type, GeneratorSettings settings, bool useList = false, bool entityAsInterface = false, string fullNamespace = null)
        {
            var simple = type as SimpleType;
            if (simple != null)
            {
                if (simple is BinaryType) return "string";
                if (simple is BooleanType) return "bool";
                if (simple is IntegerType) return "long";
                if (simple is LogicalType) return "bool?";
                if (simple is NumberType) return "double";
                if (simple is RealType) return "double";
                if (simple is StringType) return "string";
            }

            var entity = type as EntityDefinition;
            if (entity != null && entityAsInterface)
                return "I" + entity.Name;

            var select = type as SelectType;
            if (select != null && entityAsInterface)
                return "I" + select.Name;

            var named = type as NamedType;
            if (named != null) return string.IsNullOrWhiteSpace(fullNamespace) ? 
                named.Name.ToString() :
                string.Format("{0}.{1}", fullNamespace, named.Name);

            var aggr = type as AggregationType;
            if (aggr != null)
            {
                return useList ?
                    string.Format("List<{0}>", GetCSType(aggr.ElementType, settings, true, entityAsInterface, fullNamespace))
                    : string.Format("{0}<{1}>", "I" + settings.ItemSetClassName, GetCSType(aggr.ElementType, settings, false, entityAsInterface, fullNamespace));
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
