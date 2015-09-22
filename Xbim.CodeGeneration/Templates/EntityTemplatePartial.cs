using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityTemplate : ICodeTemplate
    {
        public EntityDefinition Type { get; private set; }

        protected readonly NamedTypeHelper Helper;

        protected readonly GeneratorSettings Settings;

        protected EntityTemplate(){}
        public EntityTemplate(GeneratorSettings settings, EntityDefinition type)
        {
            Settings = settings;
            Helper = new NamedTypeHelper(type, settings);
            Type = type;
            WhereRules = Type.WhereRules.ToList();
            ExplicitAttributes = Type.ExplicitAttributes.ToList();
            AllExplicitAttributes = MakeUniqueNameList(Type.AllExplicitAttributes).ToList();
            InverseAttributes = Type.InverseAttributes.ToList();
            AllInverseAttributes = MakeUniqueNameList(Type.AllInverseAttributes).ToList();
        }

        private IEnumerable<T> MakeUniqueNameList<T>(IEnumerable<T> rawAttributes) where T: Attribute
        {
            var seen = new HashSet<string>();
            foreach (var attribute in rawAttributes.Where(attribute => !seen.Contains(attribute.Name)))
            {
                seen.Add(attribute.Name);
                yield return attribute;
            }
        }

        public string Namespace
        {
            get
            {
                return Helper.FullNamespace;
            }
        }

        public bool IsAbstract { get { return !Type.Instantiable; } }

        public string Name { get { return Type.Name; } }

        public virtual string Inheritance
        {
            get
            {
                var parents = new List<string>();
                if (IsFirst)
                {
                    parents.Add(Settings.PersistEntityInterface);
                    parents.Add("INotifyPropertyChanged");
                }
                else
                    parents.AddRange(Type.Supertypes.Select(t => 
                        Settings.GenerateAllAsInterfaces ? 
                        "I" + t.Name.ToString() : 
                        t.Name.ToString()));

                //add any interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //sign types to be instantiable in entity factory
                if(Type.Instantiable)
                    parents.Add(Settings.InstantiableEntityInterface);

                //merge to a single string
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        protected virtual string GetCSType(ExplicitAttribute attribute)
        {
            return TypeHelper.GetCSType(attribute, Settings);
        }

        protected string GetAttributeState(Attribute attribute)
        {
            const string enu = "EntityAttributeState.";
            var expl = attribute as ExplicitAttribute;
            if (expl != null && expl.OptionalFlag)
                return enu + "Optional";
            if (expl != null)
                return enu + "Mandatory";

            var inverse = attribute as InverseAttribute;
            if (inverse != null)
                return enu + "Mandatory";

            var derived = attribute as DerivedAttribute;
            if (derived != null)
                return derived.Redeclaring != null ? enu + "DerivedOverride" : enu + "Derived";

            throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);
        }

        private static BaseType GetDomain(Attribute attribute)
        {
            BaseType domain = null;
            var expl = attribute as ExplicitAttribute;
            if (expl != null)
                domain = expl.Domain;
            var inverse = attribute as InverseAttribute;
            if (inverse != null)
                domain = inverse.Domain;
            var derived = attribute as DerivedAttribute;
            if (derived != null)
                domain = derived.Domain;

            return domain;
        }

        private string GetAttributeType(BaseType domain)
        {
            const string enu = "EntityAttributeType.";
            var list = domain as ListType;
            if (list != null)
                return list.UniqueFlag ? enu + "ListUnique" : enu + "List";

            if (domain is SetType)
                return enu + "Set";

            if (domain is BagType)
                return enu + "Bag";

            var arr = domain as ArrayType;
            if (arr != null)
                return arr.UniqueFlag ? enu + "ArrayUnique" : enu + "Array";

            if (domain is EntityDefinition || domain is SelectType)
                return enu + "Class";

            if (domain is SimpleType || domain is DefinedType)
                return enu + "None";

            if (domain is EnumerationType)
                return enu + "Enum";

            throw new NotSupportedException("Unexpected type " + domain.GetType());
        }

        protected string GetAttributeType(Attribute attribute)
        {
            if (attribute is InverseAttribute)
                return "EntityAttributeType.Set";

            var domain = GetDomain(attribute);

            if(domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            return GetAttributeType(domain);
        }

        protected string GetAttributeMemberType(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if(domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            var inv = attribute as InverseAttribute;
            if (inv != null)
                return GetAttributeType(inv.Domain);

            var aggr = domain as AggregationType;
            return aggr == null ? 
                "EntityAttributeType.None" : 
                GetAttributeType(aggr.ElementType);
        }

        protected int GetAttributeMinCardinality(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            var aggr = domain as VariableSizeAggregationType;
            if (aggr != null)
                return aggr.LowerBound;

            var arr = domain as ArrayType;
            if (arr != null)
                return arr.LowerIndex;

            return -1;
        }

        protected int GetAttributeMaxCardinality(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            var aggr = domain as VariableSizeAggregationType;
            if (aggr != null) 
                return aggr.UpperBound ?? -1;

            var arr = domain as ArrayType;
            if (arr != null)
                return arr.UpperIndex;

            return -1;
        }

        protected int GetAttributeOrder(Attribute attribute)
        {
            var expl = attribute as ExplicitAttribute;
            if (expl != null)
                return GetAttributeIndex(expl) + 1;

            return -1;
        }

        protected bool IsPartOfInverse(ExplicitAttribute attribute)
        {
            return Type.SchemaModel.Get<InverseAttribute>(i => i.InvertedAttr == attribute).Any();
        }

        protected bool IsEnumeration(InverseAttribute attribute)
        {
            return attribute.AggregationType != null;
        }

        protected string ModelInterface { get { return Settings.ModelInterface; } }

        protected string AbstractKeyword { get { return IsAbstract ? "abstract" : ""; } }
        protected string VirtualOverrideKeyword { get { return (IsFirst ? "virtual" : " override"); } }

        protected bool IsFirst { get { return Type.Supertypes == null || !Type.Supertypes.Any(); } }
        protected bool IsFirstNonAbstract { 
            get 
            { 
                return Type.Instantiable && (Type.Supertypes == null  ||  Type.AllSupertypes.All(t => !t.Instantiable)); 
            } 
        }

        protected string InstantiableInterface { get { return Settings.InstantiableEntityInterface; } }

        protected string GetPrivateFieldName(Attribute attribute)
        {
            string name = attribute.Name;
            return "_" + name.First().ToString().ToLower() + name.Substring(1);
        }


        protected bool IsOwnAttribute(ExplicitAttribute attribute)
        {
            
            return ExplicitAttributes.Contains(attribute);
        }

        protected IEnumerable<ExplicitAttribute> ParentAttributes { get
        {
            return AllExplicitAttributes.Where(a => !ExplicitAttributes.Contains(a));
        } } 

        protected List<ExplicitAttribute> ExplicitAttributes { get; private set; }

        protected List<ExplicitAttribute> AllExplicitAttributes { get; private set; }

        protected List<InverseAttribute> InverseAttributes { get; private set; }
        
        protected List<InverseAttribute> AllInverseAttributes { get; private set; }

        protected int GetAttributeIndex(ExplicitAttribute attribute)
        { 
            return AllExplicitAttributes.IndexOf(attribute);
        }



        public virtual IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();
                var expl =  IsAbstract ? ExplicitAttributes : AllExplicitAttributes;

                var selects = Type.IsInSelects.ToList();
                var supertypes = Type.Supertypes ?? new HashSet<EntityDefinition>();
                var eAttributes = expl.Where(a => a.Domain is NamedType).Select(a => a.Domain as NamedType).ToList();
                var eaAttributes =
                    expl.Where(a => a.Domain is AggregationType)
                        .Select(a => GetNamedElementType(a.Domain as AggregationType))
                        .Where(t => t != null).ToList();
                var iAttributes = InverseAttributes.Select(a => a.Domain).ToList();

                namedOccurances.AddRange(selects);
                namedOccurances.AddRange(supertypes);
                namedOccurances.AddRange(eAttributes);
                namedOccurances.AddRange(eaAttributes);
                namedOccurances.AddRange(iAttributes);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, Settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if(result.Contains(ns)) continue;
                    result.Add(ns);
                }

                if (iAttributes.Any() || !IsAbstract)
                    result.Add("System.Collections.Generic");

                if(InverseAttributes.Any(IsDoubleAggregation))
                    result.Add("System.Linq");

                if (IsFirst)
                {
                    //for INotifyPropertyChanged
                    result.Add("System.ComponentModel");
    
                    //for Action and Exception
                    result.Add("System");
                }

                if (Settings.IsInfrastructureSeparate)
                {
                    result.Add(Settings.InfrastructureNamespace);
                    result.Add(Settings.InfrastructureNamespace + ".Exceptions");
                }
                else
                {
                    result.Add(Settings.Namespace + ".Exceptions");
                }

                return result;
            }
        }

        private string PersistInterface { get { return Settings.PersistInterface; } }

        protected NamedType GetNamedElementType(AggregationType aggregation)
        {
            if (aggregation.ElementType is SimpleType) return null;

            var named = aggregation.ElementType as NamedType;
            if (named != null) return named;

            var aggr = aggregation.ElementType as AggregationType;
            if(aggr != null)
                return GetNamedElementType(aggr);

            throw new NotSupportedException();
        }

        protected string PersistEntityInterface { get { return Settings.PersistEntityInterface; } }

        protected bool IsAggregation(InverseAttribute attribute)
        {
            return attribute.InvertedAttr.Domain is AggregationType;
        }

        protected bool IsEntityOrSelectAggregation(ExplicitAttribute attribute)
        {
            var aggr = attribute.Domain as AggregationType;
            if (aggr == null) return false;
            return aggr.ElementType is EntityDefinition || aggr.ElementType is SelectType;
        }

        private static bool IsStringType(BaseType type)
        {
            if (type is StringType) return true;
            var defType = type as DefinedType;
            if(defType == null) return false;

            return IsStringType(defType.Domain as BaseType);
        }

        private static bool IsIntType(BaseType type)
        {
            if (type is IntegerType) return true;
            var defType = type as DefinedType;
            if (defType == null) return false;

            return IsIntType(defType.Domain as BaseType);
        }

        private static bool IsBoolType(BaseType type)
        {
            if (type is BooleanType || type is LogicalType) return true;
            var defType = type as DefinedType;
            if (defType == null) return false;

            return IsBoolType(defType.Domain as BaseType);
        }

        private static bool IsRealType(BaseType type)
        {
            if (type is RealType) return true;
            var defType = type as DefinedType;
            if (defType == null) return false;

            return IsRealType(defType.Domain as BaseType);
        }

        private static bool IsNumberType(BaseType type)
        {
            if (type is NumberType) return true;
            var defType = type as DefinedType;
            if (defType == null) return false;

            return IsNumberType(defType.Domain as BaseType);
        }

        private static bool IsBinaryType(BaseType type)
        {
            if (type is BinaryType) return true;
            var defType = type as DefinedType;
            if (defType == null) return false;

            return IsBinaryType(defType.Domain as BaseType);
        }

        public static string GetPropertyValueMember(BaseType domain)
        {
            if (domain is EntityDefinition) return "EntityVal";

            var aggr = domain as AggregationType;
            if (aggr != null) return GetPropertyValueMember(aggr.ElementType);

            var def = domain as DefinedType;
            if (def != null && def.Domain is AggregationType)
                return GetPropertyValueMember((BaseType) def.Domain);

            if (IsStringType(domain)) return "StringVal";
            if (IsIntType(domain)) return "IntegerVal";
            if (IsBoolType(domain)) return "BooleanVal";
            if (IsRealType(domain)) return "RealVal";
            if (IsNumberType(domain)) return "NumberVal";
            if (IsBinaryType(domain)) return "HexadecimalVal";

            throw new Exception("Unexpected type");
        }

        private bool IsComplexDefinedType(ExplicitAttribute attribute)
        {
            var def = attribute.Domain as DefinedType;
            if (def == null) return false;
            return def.Domain is AggregationType;
        }

        private bool IsSimpleOrDefinedTypeAggregation(ExplicitAttribute attribute)
        {
            var aggr = attribute.Domain as AggregationType;
            if (aggr == null) return false;

            return aggr.ElementType is SimpleType || aggr.ElementType is DefinedType;
        }

        private bool IsSimpleOrDefinedType(ExplicitAttribute attribute)
        { 
            var domain = attribute.Domain;
            return domain is SimpleType || domain is DefinedType;
        }

        protected bool IsDoubleAggregation(InverseAttribute attribute)
        {
            var aggr = attribute.InvertedAttr.Domain as AggregationType;
            if (aggr == null) return false;

            return aggr.ElementType is AggregationType;
        }

        protected IEnumerable<ExplicitAttribute> AggregatedExplicitAttributes
        {
            get { return ExplicitAttributes.Where(t => t.Domain is AggregationType); }
        }

        protected bool IsReferenceTypeAggregation(ExplicitAttribute attribute)
        {
            var agg = attribute.Domain as AggregationType;
            if (agg == null) return false;
            return
                agg.ElementType is EntityDefinition ||
                agg.ElementType is SelectType ||
                agg.ElementType is StringType ||
                agg.ElementType is LogicalType;
        }

        protected bool IsValueTypeAggregation(ExplicitAttribute attribute)
        {
            var agg = attribute.Domain as AggregationType;
            if (agg == null) return false;
            return
                agg.ElementType is SimpleType && 
                !(agg.ElementType is StringType ||
                agg.ElementType is LogicalType);
        }

        protected bool IsReferenceType(ExplicitAttribute attribute)
        {
            if (attribute.OptionalFlag) return true;

            return
                attribute.Domain is EntityDefinition ||
                attribute.Domain is SelectType ||
                attribute.Domain is StringType ||
                attribute.Domain is LogicalType ||
                attribute.Domain is AggregationType;
        }

        protected string GetAggregationElementType(ExplicitAttribute attribute)
        {
            var aggregationType = attribute.Domain as AggregationType;
            if (aggregationType != null)
            {
                var type = aggregationType.ElementType;
                return TypeHelper.GetCSType(type, Settings);
            }
            throw new Exception("Aggregation type expected");
        }

        public List<WhereRule> WhereRules { get; set; }
    }
}
