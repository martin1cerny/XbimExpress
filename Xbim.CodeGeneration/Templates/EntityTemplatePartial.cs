using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xbim.CodeGeneration.Helpers;
using Xbim.CodeGeneration.Settings;
using Xbim.ExpressParser.SDAI;
using Attribute = Xbim.ExpressParser.SDAI.Attribute;

namespace Xbim.CodeGeneration.Templates
{
    public partial class EntityTemplate : ICodeTemplate
    {
        public EntityDefinition Type { get; private set; }

        protected readonly NamedTypeHelper _helper;

        protected readonly GeneratorSettings _settings;

        protected EntityTemplate(){}
        public EntityTemplate(GeneratorSettings settings, EntityDefinition type)
        {
            _settings = settings;
            _helper = new NamedTypeHelper(type, settings);
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
                return _helper.FullNamespace;
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
                    if (!Type.Instantiable)  //avoid redundant inheritance
                        parents.Add(_settings.PersistEntityInterface);
                    parents.Add("INotifyPropertyChanged");
                }
                else
                    parents.AddRange(Type.Supertypes.Select(t => 
                        _settings.GenerateAllAsInterfaces ? 
                        "I" + t.Name.ToString() : 
                        t.Name.ToString()));

                //add any interfaces
                parents.AddRange(Type.IsInSelects.Select(s => s.Name.ToString()));

                //sign types to be instantiable in entity factory
                if(Type.Instantiable)
                    parents.Add(_settings.InstantiableEntityInterface);

                //merge to a single string
                var i = String.Join(", ", parents);
                if (String.IsNullOrWhiteSpace(i)) return "";
                return ": " + i;
            }
        }

        protected virtual string GetCSType(ExplicitAttribute attribute)
        {
            return TypeHelper.GetCSType(attribute, _settings);
        }

        protected string ModelInterface { get { return _settings.ModelInterface; } }

        protected string AbstractKeyword { get { return IsAbstract ? "abstract" : ""; } }
        protected string VirtualOverrideKeyword { get { return (IsAbstract ? "virtual" : "") + (IsFirst ? "" : " override"); } }

        protected bool IsFirst { get { return Type.Supertypes == null || !Type.Supertypes.Any(); } }
        protected bool IsFirstNonAbstract { 
            get 
            { 
                return Type.Instantiable && (Type.Supertypes == null  ||  Type.AllSupertypes.All(t => !t.Instantiable)); 
            } 
        }

        protected string InstantiableInterface { get { return _settings.InstantiableEntityInterface; } }

        protected string GetPrivateFieldName(Attribute attribute)
        {
            string name = attribute.Name;
            return "_" + name.First().ToString().ToLower() + name.Substring(1);
        }


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
                    var helper = new NamedTypeHelper(type, _settings);
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

                if (_settings.IsInfrastructureSeparate)
                    result.Add(_settings.InfrastructureNamespace);

                return result;
            }
        }

        private string PersistInterface { get { return _settings.PersistInterface; } }

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

        protected string PersistEntityInterface { get { return _settings.PersistEntityInterface; } }

        protected bool IsAggregation(InverseAttribute attribute)
        {
            return attribute.InvertedAttr.Domain is AggregationType;
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
                return TypeHelper.GetCSType(type, _settings);
            }
            throw new Exception("Aggregation type expected");
        }

        public List<WhereRule> WhereRules { get; set; }
    }
}
