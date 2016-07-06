using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;

namespace Xbim.CodeGeneration.Templates
{
    public partial class DefinedTypeTemplate : ICodeTemplate
    {
        public DefinedType Type { get; private set; }

        private readonly NamedTypeHelper _helper;

        private readonly GeneratorSettings _settings;
        
        public DefinedTypeTemplate(GeneratorSettings settings, DefinedType type)
        {
            _settings = settings;
            _helper = new NamedTypeHelper(type, settings);
            Type = type;
        }

        public string Namespace
        {
            get
            {
                return _helper.FullNamespace;
            }
        }

        private string UnderlyingType
        {
            get { return TypeHelper.GetCSType(Type.Domain, _settings); }
        }

        private string UnderlyingArrayType
        {
            get
            {
                var aggrType = Type.Domain as AggregationType;
                if (aggrType == null) throw new Exception("Underlying type is not an array type. This is not a complex type.");
                return TypeHelper.GetCSType(aggrType.ElementType, _settings);
            }
        }

        private string UnderlyingArrayTypeNamespace
        {
            get
            {
                var aggrType = Type.Domain as AggregationType;
                if (aggrType == null) return null;
                var namedType = aggrType.ElementType as NamedType;
                if (namedType == null) return null;
                var helper = new NamedTypeHelper(namedType, _settings);
                return helper.FullNamespace;
            }
        }

        public string Name { get { return Type.Name; } }

        private bool IsComplex
        {
            get { return Type.Domain is AggregationType; }
        }

        private bool IsComplexOfEntities
        {
            get
            {
                var aggr = Type.Domain as AggregationType;
                if (aggr == null) return false;
                return aggr.ElementType is EntityDefinition;
            }
        }

        public string Inheritance
        {
            get
            {
                var parents = Type.IsInSelects.Select(s => s.Name.ToString()).ToList();
                parents.Add(IsComplex ? "IExpressComplexType" : "IExpressValueType");

                switch (SimpleType)
                {
                    case SimpleTypeEnum.BinaryType:
                    case SimpleTypeEnum.BooleanType:
                    case SimpleTypeEnum.IntegerType:
                    case SimpleTypeEnum.LogicalType:
                    case SimpleTypeEnum.NumberType:
                    case SimpleTypeEnum.RealType:
                    case SimpleTypeEnum.StringType:
                        parents.Add("IExpress" + SimpleType);
                        break;
                }
                
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i))
                    return "";
                return ": " + i;
            }
        }

        private string PersistInterface
        {
            get { return _settings.PersistInterface; }
        }

        public IEnumerable<string> Using
        {
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();

                var selects = Type.IsInSelects.ToList();

                namedOccurances.AddRange(selects);

                var namedDomain = Type.Domain as NamedType;
                if (namedDomain != null)
                    namedOccurances.Add(namedDomain);

                if (IsComplex)
                {
                    result.Add("System.Collections.Generic");
                    result.Add("System.Linq");

                    //get base type
                    var aggrNs = UnderlyingArrayTypeNamespace;
                    if (aggrNs != null)
                        result.Add(aggrNs);
                }
                if (_settings.IsInfrastructureSeparate)
                {
                    result.Add(_settings.InfrastructureNamespace);
                    result.Add(_settings.InfrastructureNamespace + ".Exceptions");
                }
                else
                {
                    result.Add(_settings.Namespace + ".Exceptions");
                }

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, _settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                return result;
            }
        }

        private static string GetPropertyValueMember(UnderlyingType domain)
        {
            return EntityInterfaceTemplate.GetPropertyValueMember(domain as BaseType);
        }

        private SimpleTypeEnum SimpleType
        {
            get
            {
                var domain = Type.Domain;
                var simple = domain as SimpleType;
                while (simple == null)
                {
                    var defType = domain as DefinedType;
                    if (defType != null)
                        simple = defType.Domain as SimpleType;
                    else
                        break;
                }

                //special case when defined type contains enumeration
                if (domain is EnumerationType) return SimpleTypeEnum.EnumerationType;
                if(domain is AggregationType) return SimpleTypeEnum.ArrayType;

                if (simple == null) throw new Exception("Unexpected type");

                if (simple is BinaryType) return SimpleTypeEnum.BinaryType;
                if (simple is BooleanType) return SimpleTypeEnum.BooleanType;
                if (simple is IntegerType) return SimpleTypeEnum.IntegerType;
                if (simple is LogicalType) return SimpleTypeEnum.LogicalType;
                if (simple is NumberType) return SimpleTypeEnum.NumberType;
                if (simple is RealType) return SimpleTypeEnum.RealType;
                if (simple is StringType) return SimpleTypeEnum.StringType;
                throw new Exception("Unexpected type");
            }
        }
    }

    public enum SimpleTypeEnum
    {
        BinaryType = 0,
        BooleanType = 1,
        IntegerType = 2,
        LogicalType = 3,
        NumberType = 4,
        RealType = 5,
        StringType = 6,
        EnumerationType = 7,
        ArrayType = 8
    }
}
