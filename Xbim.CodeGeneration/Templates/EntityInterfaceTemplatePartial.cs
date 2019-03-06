using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Xbim.IfcDomains;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityInterfaceTemplate : ICodeTemplate
    {
        public EntityInterfaceTemplate(GeneratorSettings settings, EntityDefinition type)
        {
            Settings = settings;
            Helper = new NamedTypeHelper(type, settings);
            Type = type;
            WhereRules = Type.WhereRules.ToList();
            ExplicitAttributes = Type.ExplicitAttributes.ToList();
            AllExplicitAttributes = MakeUniqueNameList(Type.AllExplicitAttributes).ToList();
            InverseAttributes = Type.InverseAttributes.ToList();
            AllInverseAttributes = MakeUniqueNameList(Type.AllInverseAttributes).ToList();
            AllAttributes = Type.AllAttributes.Where(a => !(a is DerivedAttribute)).ToList();
        }

        // ReSharper disable once InconsistentNaming
        protected string GetInterfaceCSType(ExplicitOrDerived attribute)
        {
            while (true)
            {
                if (attribute is ExplicitAttribute expl)
                    return TypeHelper.GetInterfaceCSType(expl, Settings);
                if (!(attribute is DerivedAttribute der)) return "";

                attribute = der.Redeclaring;
            }
        }

        public string InterfaceNamespace => Settings.Namespace + "." + Settings.SchemaInterfacesNamespace;

        protected bool IsEntityDefinitionSelect(BaseType type)
        {
            return type is SelectType sel && GetAllSpecific(sel).All(t => t is EntityDefinition);
        }

        protected bool CouldBeEntityDefinitionSelect(BaseType type)
        {
            return type is SelectType sel && GetAllSpecific(sel).Any(t => t is EntityDefinition);
        }

        protected static IEnumerable<NamedType> GetAllSpecific(SelectType select)
        {
            foreach (var type in select.Selections)
            {
                if (!(type is SelectType nested))
                {
                    yield return type;
                    continue;
                }
                foreach (var namedType in GetAllSpecific(nested))
                {
                    yield return namedType;
                }
            }
        }

        protected string GetInnerType(string aggrType)
        {
            while (true)
            {
                if (aggrType.StartsWith("IItemSet"))
                {
                    aggrType = aggrType.Substring(9); //remove "IItemSet<"
                    aggrType = aggrType.Substring(0, aggrType.Length - 1); //remove ">"
                }

                if (aggrType.StartsWith("IEnumerable"))
                {
                    aggrType = aggrType.Substring(12); //remove "IEnumerable<"
                    aggrType = aggrType.Substring(0, aggrType.Length - 1); //remove ">"
                }

                if (aggrType.StartsWith("ItemSet"))
                {
                    aggrType = aggrType.Substring(8); //remove "ItemSet<"
                    aggrType = aggrType.Substring(0, aggrType.Length - 1); //remove ">"
                }

                if (aggrType.StartsWith("OptionalItemSet"))
                {
                    aggrType = aggrType.Substring(16); //remove "OptionalItemSet<"
                    aggrType = aggrType.Substring(0, aggrType.Length - 1); //remove ">"
                }

                if (aggrType.StartsWith("IOptionalItemSet"))
                {
                    aggrType = aggrType.Substring(17); //remove "OptionalItemSet<"
                    aggrType = aggrType.Substring(0, aggrType.Length - 1); //remove ">"
                }

                if (aggrType.Contains("ItemSet"))
                    continue;

                return aggrType;
            }
        }

        protected string InterfaceInheritance
        {
            get
            {
                var parents = new List<string>();
                if (IsFirst)
                    parents.Add(Settings.PersistEntityInterface);
                else
                    parents.AddRange(Type.Supertypes.Select(t => "I" + t.Name.ToString()));

                //add any select interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //merge to a single string
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        private static bool IsEntityReference(BaseType type)
        {
            if (type is EntityDefinition)
                return true;

            if (!(type is SelectType select))
                return false;

            var realTypes = SelectTypeTemplate.GetFinalTypes(select);
            return realTypes.All(t => t is EntityDefinition);
        }

        private bool IsDirectEntityRefOrAggr(ExplicitAttribute attribute)
        {
            if (OverridingAttributes.Any(a => a.Name == attribute.Name))
                return false;
            if (IsEntityReference(attribute))
                return true;
            if (!(attribute.Domain is AggregationType aggr))
                return false;
            var nt = GetNamedElementType(aggr);
            if (nt is EntityDefinition)
                return true;

            return nt is SelectType select && GetAllSpecific(@select).All(s => s is EntityDefinition);
        }

        private static bool IsEntityReference(ExplicitAttribute attribute)
        {
            return IsEntityReference(attribute.Domain);
        }

        private static bool IsEntityReferenceAggregation(BaseType type)
        {
            return type is AggregationType aggr && IsEntityReference(aggr.ElementType);
        }

        private static bool IsEntityReferenceAggregation(ExplicitAttribute attribute)
        {
            return IsEntityReferenceAggregation(attribute.Domain);
        }

        private static bool IsEntityReferenceDoubleAggregation(BaseType type)
        {
            return type is AggregationType aggr && IsEntityReferenceAggregation(aggr.ElementType);
        }

        private static bool IsEntityReferenceDoubleAggregation(ExplicitAttribute attribute)
        {
            return IsEntityReferenceDoubleAggregation(attribute.Domain);
        }

        public EntityDefinition Type { get; }

        protected readonly NamedTypeHelper Helper;

        protected readonly GeneratorSettings Settings;


        private IEnumerable<T> MakeUniqueNameList<T>(IEnumerable<T> rawAttributes) where T : Attribute
        {
            var seen = new HashSet<string>();
            foreach (var attribute in rawAttributes.Where(attribute => !seen.Contains(attribute.Name)))
            {
                seen.Add(attribute.Name);
                yield return attribute;
            }
        }

        public virtual string Namespace => Helper.FullNamespace;

        public bool IsAbstract => !Type.Instantiable;

        public virtual string Name => Type.Name;

        public virtual string Inheritance
        {
            get
            {
                var parents = new List<string>();
                if (IsFirst)
                {
                    parents.Add("PersistEntity");
                }
                else
                    parents.AddRange(Type.Supertypes.Select(t => t.Name.ToString()));


                //sign types to be instantiable in entity factory
                if (Type.Instantiable)
                    parents.Add(Settings.InstantiableEntityInterface);

                //add own interface
                if (Settings.GenerateInterfaces)
                    parents.Add("I" + Name);
                else
                    //add any select interfaces
                    parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));


                if (Type.Instantiable && AllExplicitAttributes.Any(IsDirectEntityRefOrAggr))
                {
                    parents.Add("IContainsEntityReferences");

                    var indexedAttributes = AllExplicitAttributes.Where(IsPartOfInverse).ToList();
                    if (indexedAttributes.Any())
                        parents.Add("IContainsIndexedReferences");
                }

                //merge to a single string
                var i = string.Join(", ", parents);
                if (string.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        protected virtual string GetCSType(ExplicitOrDerived attribute)
        {
            while (true)
            {
                if (attribute is ExplicitAttribute expl)
                    return TypeHelper.GetCSType(expl, Settings);
                var der = attribute as DerivedAttribute;
                if (der != null && der.Redeclaring == null)
                {
                    var result = TypeHelper.GetCSType(der.Domain, Settings, true);
                    result = TweekDerivedType(der, result);
                    return result;
                }
                if (der != null)
                    attribute = der.Redeclaring;
            }
        }

        protected readonly Dictionary<string, string> SpecialDerivesDictionary = new Dictionary<string, string>
        {
            {"IIfcDirection", "Common.Geometry.XbimVector3D"},
            {"IfcDirection", "Common.Geometry.XbimVector3D"},
            {"IIfcVector", "Common.Geometry.XbimVector3D"},
            {"IfcVector", "Common.Geometry.XbimVector3D"},
            {"IIfcCartesianPoint", "Common.Geometry.XbimPoint3D"},
            {"IfcCartesianPoint", "Common.Geometry.XbimPoint3D"},
            {"IIfcLine", "Common.Geometry.XbimLine"},
            {"IfcLine", "Common.Geometry.XbimLine"},
            {"IIfcDimensionalExponents", "Common.Geometry.XbimDimensionalExponents"},
            {"IfcDimensionalExponents", "Common.Geometry.XbimDimensionalExponents"},
        };

        protected virtual string TweekDerivedType(DerivedAttribute attribute, string type)
        {
            var domain = attribute.Domain;
            if (domain is AggregationType aggr)
            {
                //drill down
                while (aggr != null)
                {
                    domain = aggr.ElementType;
                    aggr = domain as AggregationType;
                }
            }
            if (!(domain is EntityDefinition))
                return type;

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var kvp in SpecialDerivesDictionary)
            {
                type = type.Replace(kvp.Key, kvp.Value);
            }
            return type;
        }

        protected bool IsOverridenAttribute(ExplicitAttribute attribute)
        {
            return Type.SchemaModel.Get<DerivedAttribute>(d => d.Redeclaring == attribute).Any();
        }

        protected string GetDerivedKeyword(DerivedAttribute attribute)
        {
            var overrides = Type.AllSupertypes.Any(s => s.DerivedAttributes.Any(d => d.Name == attribute.Name));
            if (overrides) return "override ";

            var virt = Type.AllSubTypes.Any(s => s.DerivedAttributes.Any(d => d.Name == attribute.Name));
            if (virt) return "virtual ";

            return "";
        }

        protected bool IsIgnored(DerivedAttribute attribute)
        {
            var type = attribute.ParentEntity;
            if (Settings.IgnoreDerivedAttributes == null || !Settings.IgnoreDerivedAttributes.Any())
                return false;

            if (type.IsInAllSelects.Any(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0)))
                return true;

            return type.AllSupertypes.Any(s => Settings.IgnoreDerivedAttributes.Any(i =>
                string.Compare(i.EntityName, s.Name, StringComparison.InvariantCultureIgnoreCase) == 0 &&
                string.Compare(i.Name, attribute.Name, StringComparison.InvariantCultureIgnoreCase) == 0));
        }

        protected bool IsOverwritting(DerivedAttribute attribute)
        {
            return attribute.ParentEntity.AllSupertypes.Any(s => s.DerivedAttributes.Any(d => d.Name == attribute.Name));
        }

        protected string GetDerivedAccess(DerivedAttribute attribute)
        {
            if (attribute.AccessCandidates == null || !attribute.AccessCandidates.Any())
                return null;

            if (string.IsNullOrWhiteSpace(attribute.AccessFunction))
                return "return " + string.Join(" ?? ", attribute.AccessCandidates.Select(c => string.Join(".", c))) + ";";

            var singleAccess = string.Join(".", attribute.AccessCandidates.First());
            return $"return {attribute.AccessFunction}({singleAccess});";
        }

        // ReSharper disable once InconsistentNaming
        protected virtual string GetCSTypeNN(ExplicitAttribute attribute)
        {
            var result = TypeHelper.GetCSType(attribute, Settings);
            return result.Trim('?');
        }

        protected int GetUpperBound(ExplicitAttribute attribute)
        {
            var aggr = attribute.Domain as VariableSizeAggregationType;
            if (aggr?.UpperBound != null && aggr.UpperBound.Value > 0)
                return aggr.UpperBound ?? -1;
            if (attribute.Domain is ArrayType arr && arr.UpperIndex > 0)
                return arr.UpperIndex;
            return 0;
        }

        protected IEnumerable<DerivedAttribute> OverridingAttributes
        {
            get { return Type.Attributes.OfType<DerivedAttribute>().Where(da => da.Redeclaring != null); }
        }

        protected IEnumerable<DerivedAttribute> DerivedAttributes
        {
            get { return Type.Attributes.OfType<DerivedAttribute>().Where(da => da.Redeclaring == null); }
        }

        protected string GetAttributeState(Attribute attribute)
        {
            const string enu = "EntityAttributeState.";
            var expl = attribute as ExplicitAttribute;
            if (expl != null && expl.OptionalFlag)
                return enu + "Optional";
            if (expl != null)
                return enu + "Mandatory";

            if (attribute is InverseAttribute inverse)
                return enu + "Mandatory";

            if (attribute is DerivedAttribute derived)
                return derived.Redeclaring != null ? enu + "DerivedOverride" : enu + "Derived";

            throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);
        }

        protected static BaseType GetDomain(Attribute attribute)
        {
            BaseType domain = null;
            if (attribute is ExplicitAttribute expl)
                domain = expl.Domain;
            if (attribute is InverseAttribute inverse)
                domain = inverse.Domain;
            if (attribute is DerivedAttribute derived)
                domain = derived.Domain;

            return domain;
        }

        protected string GetAttributeType(BaseType domain)
        {
            const string enu = "EntityAttributeType.";
            if (domain is ListType list)
                return list.UniqueFlag ? enu + "ListUnique" : enu + "List";

            if (domain is SetType)
                return enu + "Set";

            if (domain is BagType)
                return enu + "Bag";

            if (domain is ArrayType arr)
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

            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            return GetAttributeType(domain);
        }

        protected string GetAttributeMemberType(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            if (attribute is InverseAttribute inv)
                return GetAttributeType(inv.Domain);

            return !(domain is AggregationType aggr) ?
                "EntityAttributeType.None" :
                GetAttributeType(aggr.ElementType);
        }

        protected string GetAttributeMinCardinality(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            if (attribute is InverseAttribute inverse)
                domain = inverse.AggregationType;

            //collect all levels of the aggregation
            if (!(domain is AggregationType enumType))
                return "null";
            var cardinality = new List<int>();
            while (enumType is AggregationType)
            {
                if (enumType is VariableSizeAggregationType aggr)
                    cardinality.Add(aggr.LowerBound);
                else if (enumType is ArrayType arr)
                    cardinality.Add(arr.LowerIndex);
                else
                    cardinality.Add(-1);

                enumType = enumType.ElementType as AggregationType;
            }

            return $"new int [] {{ {string.Join(", ", cardinality)} }}";
        }

        protected string GetAttributeMaxCardinality(Attribute attribute)
        {
            var domain = GetDomain(attribute);
            if (domain == null)
                throw new NotSupportedException("Unexpected type or configuration of attribute " + attribute.Name);

            if (attribute is InverseAttribute inverse)
                domain = inverse.AggregationType;

            //collect all levels of the aggregation
            if (!(domain is AggregationType enumType))
                return "null";
            var cardinality = new List<int>();
            while (enumType is AggregationType)
            {
                if (enumType is VariableSizeAggregationType aggr)
                    cardinality.Add(aggr.UpperBound ?? -1);
                else if (enumType is ArrayType arr)
                    cardinality.Add(arr.UpperIndex);
                else
                    cardinality.Add(-1);

                enumType = enumType.ElementType as AggregationType;
            }

            return $"new int [] {{ {string.Join(", ", cardinality)} }}";
        }

        protected int GetAttributeOrder(Attribute attribute)
        {
            if (attribute is ExplicitOrDerived expl)
                return GetAttributeIndex(expl) + 1;
            return -1;
        }

        protected int GetAttributeGlobalOrder(Attribute attribute)
        {

            if (attribute is DerivedAttribute) return 0;
            return AllAttributes.IndexOf(attribute) + 1;
        }

        protected bool IsPartOfInverse(ExplicitAttribute attribute)
        {
            return Type.SchemaModel.Get<InverseAttribute>(i => i.InvertedAttr == attribute).Any();
        }

        protected bool IsEnumeration(InverseAttribute attribute)
        {
            return attribute.AggregationType != null;
        }

        protected string ModelInterface => Settings.ModelInterface;

        protected string AbstractKeyword => IsAbstract ? "abstract" : "";
        protected string VirtualOverrideKeyword => (IsFirst ? "virtual" : " override");

        protected bool IsFirst => Type.Supertypes == null || !Type.Supertypes.Any();

        protected bool IsFirstNonAbstract
        {
            get
            {
                return Type.Instantiable && (Type.Supertypes == null || Type.AllSupertypes.All(t => !t.Instantiable));
            }
        }

        protected string InstantiableInterface => Settings.InstantiableEntityInterface;

        protected string GetPrivateFieldName(Attribute attribute)
        {
            string name = attribute.Name;
            return "_" + name.First().ToString().ToLower() + name.Substring(1);
        }


        protected bool IsOwnAttribute(ExplicitAttribute attribute)
        {

            return ExplicitAttributes.Contains(attribute);
        }

        protected IEnumerable<ExplicitAttribute> ParentAttributes
        {
            get
            {
                return AllExplicitAttributes.Where(a => !ExplicitAttributes.Contains(a));
            }
        }

        protected List<ExplicitAttribute> ExplicitAttributes { get; }

        protected List<ExplicitAttribute> AllExplicitAttributes { get; }

        protected List<InverseAttribute> InverseAttributes { get; }

        protected List<InverseAttribute> AllInverseAttributes { get; private set; }

        protected List<Attribute> AllAttributes { get; }

        protected int GetAttributeIndex(ExplicitOrDerived attribute)
        {
            while (true)
            {
                if (attribute is ExplicitAttribute expl)
                    return AllExplicitAttributes.IndexOf(expl);
                if (!(attribute is DerivedAttribute derived)) return -1;
                attribute = derived.Redeclaring;
            }
        }


        public virtual IEnumerable<string> Using
        {
            //need to add namespaces for all inheritance and attributes
            get
            {
                var result = new List<string>();
                var namedOccurances = new List<NamedType>();
                var expl = ExplicitAttributes;

                var selects = Type.IsInSelects.ToList();
                var supertypes = Type.Supertypes ?? new HashSet<EntityDefinition>();
                var eAttributes = expl.Where(a => a.Domain is NamedType).Select(a => a.Domain as NamedType).ToList();
                var eaAttributes =
                    expl.Where(a => a.Domain is AggregationType)
                        .Select(a => GetNamedElementType(a.Domain as AggregationType))
                        .Where(t => t != null).ToList();
                var iAttributes = InverseAttributes.Select(a => a.Domain).ToList();
                var dAttributes = Type.DerivedAttributes.Where(d => d.Domain is NamedType).Select(d => d.Domain as NamedType);

                namedOccurances.AddRange(selects);
                namedOccurances.AddRange(supertypes);
                namedOccurances.AddRange(eAttributes);
                namedOccurances.AddRange(eaAttributes);
                namedOccurances.AddRange(iAttributes);
                namedOccurances.AddRange(dAttributes);

                foreach (var type in namedOccurances)
                {
                    var helper = new NamedTypeHelper(type, Settings);
                    var ns = helper.FullNamespace;
                    if (ns == Namespace) continue;
                    if (result.Contains(ns)) continue;
                    result.Add(ns);
                }

                result.Add("System");
                result.Add("System.Collections.Generic");

                result.Add("System.Linq");

                if (IsFirst)
                {
                    //for INotifyPropertyChanged
                    result.Add("System.ComponentModel");
                    result.Add(Settings.InfrastructureNamespace + ".Metadata");

                }

                result.Add(Settings.InfrastructureNamespace);
                result.Add(Settings.InfrastructureNamespace + ".Exceptions");


                result.Add(InterfaceNamespace);
                result.Add(Namespace);

                return result.Distinct();
            }
        }

        protected string GetFullNamespace(BaseType type, string mainNamespace, DomainStructure structure)
        {
            while (true)
            {
                if (structure == null)
                    return mainNamespace;

                if (type is SimpleType)
                    return "System";

                if (type is NamedType namedType)
                {
                    var domain = structure.GetDomainForType(namedType.Name);
                    return domain != null ?
                        $"{mainNamespace}.{domain.Name}"
                        :
                        mainNamespace;

                }
                if (type is AggregationType aggr)
                {
                    type = aggr.ElementType;
                    continue;
                }
                throw new Exception("Unexpected type");
            }

        }

        protected string PersistInterface => Settings.PersistInterface;

        protected NamedType GetNamedElementType(AggregationType aggregation)
        {
            if (aggregation.ElementType is SimpleType) return null;

            if (aggregation.ElementType is NamedType named) return named;

            if (aggregation.ElementType is AggregationType aggr)
                return GetNamedElementType(aggr);

            throw new NotSupportedException();
        }

        protected string PersistEntityInterface => Settings.PersistEntityInterface;

        protected bool IsAggregation(ExplicitAttribute attribute)
        {
            return attribute.Domain is AggregationType;
        }

        protected bool IsAggregation(InverseAttribute attribute)
        {
            return attribute.InvertedAttr.Domain is AggregationType;
        }

        protected bool CanBeNull(ExplicitAttribute attribute)
        {
            if (attribute.Domain is EntityDefinition)
                return true;
            if (attribute.Domain is SelectType)
                return true;
            if (IsStringType(attribute.Domain))
                return true;
            if (attribute.OptionalFlag)
                return true;
            return false;
        }

        protected bool IsEntityOrSelect(ExplicitAttribute attribute)
        {
            return attribute.Domain is EntityDefinition || attribute.Domain is SelectType;
        }

        protected bool IsEntityOrSelectAggregation(ExplicitAttribute attribute)
        {
            if (!(attribute.Domain is AggregationType aggr)) return false;
            return aggr.ElementType is EntityDefinition || aggr.ElementType is SelectType;
        }

        protected static bool IsStringType(BaseType type)
        {
            while (true)
            {
                if (type is StringType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        protected static bool IsIntType(BaseType type)
        {
            while (true)
            {
                if (type is IntegerType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        protected static bool IsBoolType(BaseType type)
        {
            while (true)
            {
                if (type is BooleanType || type is LogicalType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        protected static bool IsRealType(BaseType type)
        {
            while (true)
            {
                if (type is RealType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        protected static bool IsNumberType(BaseType type)
        {
            while (true)
            {
                if (type is NumberType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        protected static bool IsBinaryType(BaseType type)
        {
            while (true)
            {
                if (type is BinaryType) return true;
                if (!(type is DefinedType defType)) return false;

                type = defType.Domain as BaseType;
            }
        }

        public static string GetPropertyValueMember(BaseType domain)
        {
            while (true)
            {
                if (domain is EntityDefinition) return "EntityVal";

                if (domain is AggregationType aggr)
                {
                    domain = aggr.ElementType;
                    continue;
                }

                var def = domain as DefinedType;
                if (def?.Domain is AggregationType)
                {
                    domain = (BaseType)def.Domain;
                    continue;
                }

                if (IsStringType(domain)) return "StringVal";
                if (IsIntType(domain)) return "IntegerVal";
                if (IsBoolType(domain)) return "BooleanVal";
                if (IsRealType(domain)) return "RealVal";
                if (IsNumberType(domain)) return "NumberVal";
                if (IsBinaryType(domain)) return "HexadecimalVal";

                if (domain is EnumerationType) return "EnumVal";

                throw new Exception("Unexpected type");
            }
        }

        protected bool IsComplexDefinedType(ExplicitAttribute attribute)
        {
            var def = attribute.Domain as DefinedType;
            return def?.Domain is AggregationType;
        }

        protected bool IsSimpleOrDefinedTypeAggregation(ExplicitAttribute attribute)
        {
            if (!(attribute.Domain is AggregationType aggr)) return false;

            return aggr.ElementType is SimpleType || aggr.ElementType is DefinedType;
        }

        protected bool IsNestedAggregation(ExplicitAttribute attribute)
        {
            var aggr = attribute.Domain as AggregationType;

            return aggr?.ElementType is AggregationType;
        }

        protected int GetLevelOfNesting(ExplicitAttribute attribute)
        {
            if (!(attribute.Domain is AggregationType aggr)) throw new Exception("This is not a nested list attribute.");
            var level = -1;
            while (aggr != null)
            {
                level++;
                aggr = aggr.ElementType as AggregationType;
            }
            return level;
        }

        protected bool IsSimpleOrDefinedType(ExplicitAttribute attribute)
        {
            var domain = attribute.Domain;
            return domain is SimpleType || domain is DefinedType;
        }

        protected bool IsDoubleAggregation(InverseAttribute attribute)
        {
            var aggr = attribute.InvertedAttr.Domain as AggregationType;

            return aggr?.ElementType is AggregationType;
        }

        protected IEnumerable<ExplicitAttribute> AggregatedExplicitAttributes
        {
            get { return ExplicitAttributes.Where(t => t.Domain is AggregationType); }
        }

        protected bool IsReferenceTypeAggregation(ExplicitAttribute attribute)
        {
            if (!(attribute.Domain is AggregationType agg)) return false;
            return
                agg.ElementType is EntityDefinition ||
                agg.ElementType is SelectType ||
                agg.ElementType is StringType ||
                agg.ElementType is LogicalType;
        }

        protected bool IsValueTypeAggregation(ExplicitAttribute attribute)
        {
            if (!(attribute.Domain is AggregationType agg)) return false;
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
            while (aggregationType != null)
            {
                var type = aggregationType.ElementType;
                aggregationType = aggregationType.ElementType as AggregationType;
                if (aggregationType == null)
                {
                    return TypeHelper.GetCSType(type, Settings);
                }
            }
            throw new Exception("Aggregation type expected");
        }


        public List<WhereRule> WhereRules { get; set; }
    }
}
